using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WesternLib;

namespace LevelEditor
{
    public enum EditorMode
    {
        Paint,
        Select,
        Collision
    };
     
    public partial class CustomGLControl : GLControl

    {
        Color _clearColor;
        World _world;
        private MouseEventArgs _lastMouseEvent = new MouseEventArgs(MouseButtons.None, 0,0,0,0);
        private Point _topLeftCoord = new Point();
        private Tileset _selectedTileset = null;
        readonly int TILE_SIZE = 32;
        private Tile _selectedTile = null;
        private EditorMode _mode = EditorMode.Paint;
        private int _collisionNum = 0;
        private CommandHistory _history = new CommandHistory();
        public int SelectedLayer { get; set; }

        public EditorMode Mode { get { return _mode; } }
        public int CollisionNum { get { return _collisionNum; } set { _collisionNum = value; } }
        private LevelData Level { get { return _world.CurrentLevel; } }
        public delegate void MouseOverTileChangedHandler(object sender, Point location);
        public event MouseOverTileChangedHandler MouseOverTile;
        internal CommandHistory History { get { return _history; } }
      
        public CustomGLControl()
            : base(new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(), 16, 16, 0, new OpenTK.Graphics.ColorFormat(), 2))
        {
            InitializeComponent();
        }

        public Color ClearColor
        {
            get { return _clearColor; }
            set
            {
                _clearColor = value;

                if (!this.DesignMode)
                {
                    MakeCurrent();
                    GL.ClearColor(_clearColor);
                }
            }
        }

        private void CustomGLControl_Load(object sender, EventArgs e)
        {

        }

        private void CustomGLControl_Paint(object sender, PaintEventArgs e)
        {
           // base.OnPaint(e);

            if (!this.DesignMode)
            {
                MakeCurrent();

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                Tile selector = _world.ResourceManager.Tiles["tileselector"];
                Tile redsquare = _world.ResourceManager.Tiles["redsquare"];
                if (_mode == EditorMode.Select)
                {
                    _world.Camera.Ready3D();
                    GL.LoadIdentity();
                    GL.Color4(1f, 1f, 1f, 1f);
                    Vector4 screenBounds = _world.Camera.Viewport;
                    Tile fullTile = _world.ResourceManager.GetFullTile(_selectedTileset);
                    fullTile.Draw(new Vector3(screenBounds.X, screenBounds.Z+fullTile.Height, 0), 0);
                    Vector2 offset = _world.Camera.ScreenToGLTileOffset(_lastMouseEvent.X, _lastMouseEvent.Y);
                    Vector3 offset3 =VectorExtensions.LevelToGraphicsCoordinates(offset);
                    selector.Draw(new Vector3(offset3.X, offset3.Y+1, -99.9f), 0);
                }
                else
                {
                    if (_world != null)
                    {

                        _world.Draw3D(1);
                        Point pos = _world.Camera.ToTileCoordinates(_lastMouseEvent.Location).ToPoint();
                        Vector2 offset = new Vector2(pos.X, pos.Y + 1);

                        GL.Color4(1f, 1f, 1f, 1f);
                        selector.Draw(new Vector3(offset.X, offset.Y, -99.9f), 0);
                        if (_selectedTile != null && Mode == EditorMode.Paint)
                        {
                            GL.Color4(1f, 1, 1, 0.5);
                            _selectedTile.Draw(new Vector3(offset.X, offset.Y, -99.89f), 0);

                            GL.Color4(1f, 1f, 1f, 1f);
                        }
                        GL.LoadIdentity();
                        Vector2 uiloc = _world.Camera.ScreenToGraphicCoordinates(1, 33);
                        if (_selectedTile != null && Mode == EditorMode.Paint)
                        {
                            _selectedTile.Draw(new Vector3(uiloc.X, uiloc.Y, -99.99f), 0);
                            uiloc = _world.Camera.ScreenToGraphicCoordinates(0, 34);
                            redsquare.Draw(new Vector3(uiloc.X, uiloc.Y, -99.9f), 0);
                        }

                    }
                }

                SwapBuffers();
            }
        }

        internal void Test(World world)
        {
            _world = world;
           CustomGLControl_Resize(null, null);
        }

        private void CustomGLControl_Resize(object sender, EventArgs e)
        {
            if (DesignMode || _world==null)
                return;
            
            Control s = (Control)sender;
            int newWidth = s==null ? Width : s.Width;
            int newHeight = s==null ? Height : s.Height;
            _world.Camera.SetScreenDimensions(newWidth, newHeight);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            Invalidate();
        }

        private void CustomGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if(Mode == EditorMode.Select)
                TilesetMousemove(sender, e);
            Func<Vector3, Vector3> screenToGameOffset = (v) => v * (1 / (float)TILE_SIZE);

            if (_world == null)
                return;
            if (e.Button == MouseButtons.Right)
            {
                Vector3 delta = new Vector3(_lastMouseEvent.X-e.X, e.Y-_lastMouseEvent.Y, 0);
                //Console.WriteLine(delta);
                _world.Camera.Offset(screenToGameOffset(delta));
                Invalidate();
            }
            Vector2 mouseVector = _world.Camera.ToTileCoordinates(e.Location);
            Point loc = mouseVector.ToPoint();

            Point lastHoverTile = _world.Camera.ToTileCoordinates((_lastMouseEvent ?? e).Location).ToPoint();
            if (loc != lastHoverTile && MouseOverTile != null)
                MouseOverTile(this, loc);

            if (e.Button == MouseButtons.Left)
            {
                // _world.CurrentLevel.SetTile(_selectedTile, loc.X, loc.Y, SelectedLayer);
                if(Mode == EditorMode.Paint)
                    _history.AddAndExecute(new ChangeTileCommand(Level, _selectedTile, loc.X, loc.Y, SelectedLayer));
                if (Mode == EditorMode.Collision)
                {
                    PaintCollision(mouseVector);
                }
                Invalidate();
            }
            Invalidate();
            _lastMouseEvent = e;
        }

        private void PaintCollision(Vector2 mousePos)
        {
            Point mouseTile = mousePos.ToPoint();
            Vector2 remainder = mousePos - mouseTile.ToVector2();
            
            int num = 0;
            if (Control.ModifierKeys == Keys.Shift)
                num = 1;
            else if (Control.ModifierKeys == Keys.Control)
                num = 0;
            else if (remainder.X < 0.5)
            {
                if (remainder.Y < 0.5)
                    num = 2;
                else
                    num = 3;
            }
            else
            {
                if (remainder.Y < 0.5)
                    num = 5;
                else
                    num = 4;
            }

            if(num ==0 && Control.ModifierKeys != Keys.Control)
            {
                num = (Level.GetCollision(mouseTile) == 0 ? 1 : 0);
            }

                    
            _history.AddAndExecute(new ChangeCollisionCommand(Level, num, mouseTile.X, mouseTile.Y));
        }

        private void TilesetMousemove(object sender, MouseEventArgs e)
        {
            _lastMouseEvent = e;

            Invalidate();
        }


        internal void SelectTile(string tileName)
        {
            if (tileName == "ERASE")
                _selectedTile = null;
            else if (tileName == "AUTO")
            {
                Point offset = GetTileUnderMouse(_lastMouseEvent);
                _mode = EditorMode.Paint;

                _selectedTile = _world.ResourceManager.GetAutoTile(_selectedTileset, offset.X, offset.Y, 32, 32);
                Invalidate();
                Invalidate();
            }
            else
            {
                _selectedTile = _world.ResourceManager.Tiles[tileName];
                Invalidate();
            }

        }

        private Point GetTileUnderMouse(MouseEventArgs mouseEvent)
        {
            int xOffset = _lastMouseEvent.X / GameConstants.TILE_SIZE * GameConstants.TILE_SIZE;
            int yOffset = _lastMouseEvent.Y / GameConstants.TILE_SIZE * GameConstants.TILE_SIZE;
            return new Point(xOffset, yOffset);
        }

        internal void ToggleCollisionMode()
        {
            _mode = _mode == EditorMode.Collision ? EditorMode.Paint : EditorMode.Collision;
            Invalidate();
        }

        internal void ShowTileset(string tilesetName)
        {

            _selectedTileset = _world.CurrentLevel.GetAutoTileset(tilesetName) ;
            _mode = EditorMode.Select;
            Invalidate();
        }



        internal void PickTile()
        {
            Point loc = _world.Camera.ToTileCoordinates(_lastMouseEvent.Location).ToPoint();
            Tile tileUnderCursor = null;
            for (int i = SelectedLayer; i >= 0 && tileUnderCursor==null; i--)
            {
                tileUnderCursor = Level.GetTile(loc.X, loc.Y, i);
            }
            _selectedTile = tileUnderCursor;
            Invalidate();
        }

        private void CustomGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (Mode == EditorMode.Select)
            {
                _topLeftCoord = GetTileUnderMouse(e);
            }

            CustomGLControl_MouseMove(sender, e);
        }

        private void CustomGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Mode == EditorMode.Select)
            {

                Point bottomRight = GetTileUnderMouse(e);
                _selectedTile = _world.ResourceManager.GetAutoTile(_selectedTileset,
                    _topLeftCoord.X,
                    _topLeftCoord.Y, 
                    bottomRight.X - _topLeftCoord.X+GameConstants.TILE_SIZE, 
                    (bottomRight.Y-_topLeftCoord.Y)+GameConstants.TILE_SIZE
                );
                _mode = EditorMode.Paint;
            }
        }
    }
}
