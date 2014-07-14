using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WesternLib;

namespace LevelEditor
{
    public partial class MainForm : Form
    {
        World _world;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _world = new World();
            _world.Init();
            _world.ChangeLevel("default.xml");
            Activate();
            customGLControl1.Test(_world);

            var tilesets = _world.ResourceManager.Tilesets;
            ImageList il = new ImageList();
            _tileSelector.LargeImageList = il;
            _tileSelector.Items.Add("ERASE", "ERASE");
            foreach (Tileset tileset in tilesets.Values)
            {
                Bitmap bitmap = new Bitmap(tileset.Filename);
                foreach (TileData tile in tileset.Tiles)
                {
                    Image tileImage = (Image)bitmap.Clone(new Rectangle(tile.OffsetX, tile.OffsetY, tile.Width, tile.Height), PixelFormat.Format32bppArgb);
                    il.Images.Add(tile.Name, tileImage);
                    _tileSelector.Items.Add(tile.Name, tile.Name);
                }
                bitmap.Dispose();

            }
            foreach(Tileset tileset in TileLoader.LoadAutoTilesets().Values)
            {
                _tileFileList.Items.Add(tileset.Filename);
            }
            _tileSelector_SelectedIndexChanged(this,new EventArgs());
            _tileSelector.Items[0].Selected = true;

            //DirectoryInfo tilefileDir = new DirectoryInfo("graphics/tilefiles");
            //foreach(FileInfo file in tilefileDir.GetFiles("*.png"))
            //{
                
            //}
        }

        private void customGLControl1_Click(object sender, EventArgs e)
        {
        }

        private void _tileSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_tileSelector.SelectedItems.Count == 0)
                return;
            ListViewItem selectedItem = _tileSelector.SelectedItems[0];
            
            selectedItem.EnsureVisible();
            customGLControl1.SelectTile(selectedItem.Text);


        }

        private void customGLControl1_MouseOverTile(object sender, Point location)
        {
            _statusBar.Items["tileCoordinates"].Text = location.ToString();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Level files|*.xml";
            dialog.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "levels");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _world.ChangeLevel(dialog.FileName);
                Invalidate(true);
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _world.CurrentLevel.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "xml";
            dialog.Filter = "Level files|*.xml";
            dialog.AddExtension = true;
            dialog.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "levels");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(dialog.FileName);
                _world.CurrentLevel.Save(dialog.FileName);
            }
        }

       

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode > Keys.D0 && e.KeyCode <= Keys.D9)
            {
                int num = (int)e.KeyCode - (int)Keys.D0;
                if (customGLControl1.Mode == EditorMode.Paint)
                {
                    if (num > _world.CurrentLevel.Layers)
                        return;
                    customGLControl1.SelectedLayer = num - 1;
                    _statusBar.Items["layerNumber"].Text = "Layer: " + (customGLControl1.SelectedLayer + 1);
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                if(customGLControl1.Mode == EditorMode.Paint)
                    _tileSelector.Items[0].Selected = true;
                if (customGLControl1.Mode == EditorMode.Collision)
                    customGLControl1.CollisionNum = 0;
            }
            if (e.KeyCode == Keys.E)
            {
                customGLControl1.PickTile();
            }
            if (e.KeyCode == Keys.Space)
            {
                if (customGLControl1.Mode == EditorMode.Select)
                    customGLControl1.SelectTile("AUTO");
                else if(_tileFileList.SelectedIndices.Count > 0)
                    tileFileList_SelectedIndexChanged(this, new EventArgs());
                UpdateModeStatus();
            }
            if (e.KeyCode == Keys.C || e.KeyCode == Keys.J)
            {
                customGLControl1.ToggleCollisionMode();
                UpdateModeStatus();
            }
            Action<int,int> MoveCamera = (x,y) => {_world.Camera.Offset(x,y); customGLControl1.Invalidate();};
            if (e.KeyCode == Keys.Left)
                MoveCamera(-1, 0);
            if (e.KeyCode == Keys.Right)
                MoveCamera(1, 0);
            if(e.KeyCode == Keys.Up)
                MoveCamera(0, 1);
            if (e.KeyCode == Keys.Down)
                MoveCamera(0, -1);
            e.Handled = true;
        }

        private void UpdateModeStatus()
        {
            string description = customGLControl1.Mode.ToString();
            _statusBar.Items["mode"].Text = "Mode: " + description;
        }

        private void tileFileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_tileFileList.SelectedItems.Count == 0)
                return;
            var selectedItem = _tileFileList.SelectedItems[0];
            if(selectedItem != null)
                customGLControl1.ShowTileset(selectedItem.Text);

            
        }

        private void customGLControl1_Load(object sender, EventArgs e)
        {

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customGLControl1.History.Undo();
            customGLControl1.Invalidate();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customGLControl1.History.Redo();
            customGLControl1.Invalidate();
        }

        private void padToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PaddingDialog dialog = new PaddingDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    _world.CurrentLevel.Pad(dialog.Rectangle);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to create a new level?");
            if(result == DialogResult.OK)
                _world.ChangeLevel(null);
        }
    }
}
