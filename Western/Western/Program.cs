// Released to the public domain. Use, modify and relicense at will.

using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using WesternLib;
using System.Windows.Forms;
using System.IO;
using QuickFont;

namespace Western
{
    class Game : GameWindow
    {
        World _world;
        
        public World World { get { return _world; } }
        
        public Game()
            : base(800,600)
        {
            _world = new World();
            Title = "Mostacho";
            _world.Options.DrawCollision = false;
            _world.Options.DrawHitbox = false;
            _world.Options.DrawTriggers = false;
            Console.WriteLine(GL.GetString(StringName.Version));
            VSync = VSyncMode.On;
            Mouse.ButtonDown += Mouse_ButtonDown;
            Keyboard.KeyDown += Keyboard_KeyDown;
            WindowBorder = OpenTK.WindowBorder.Resizable;
            
        }

        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            
            if (e.Key == Key.F1)
                _world.Options.DrawCollision.Toggle();
            if (e.Key == Key.C && _world.State == GameState.Dialog)
                _world.GraphicsEngine.Dialog.Abort();
            else if (e.Key == Key.F2)
                _world.Options.DrawTriggers.Toggle();
            else if (e.Key == Key.F3)
                _world.Options.DrawHitbox.Toggle();
            else if (e.Key == Key.F5)
                _world.Player.Health = 1f;
            else if (e.Key == Key.L)
                ChangeLevel();
            else if (e.Key == Key.R)
                _world.Player.Equipped.Reload();
            else if (e.Key == Key.F11)
                _world.Player.MaxVelocity -= 0.1f;
            else if (e.Key == Key.F12)
                _world.Player.MaxVelocity += 0.1f;
            else if (e.Key == Key.Space)
                if (_world.State == GameState.Game)
                    _world.Player.Shoot();
                else
                    _world.GraphicsEngine.Dialog.MoveNext();
        }

        void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            Vector2 mousePos = World.Camera.ToTileCoordinates(Mouse.X, Mouse.Y);
            if (Mouse[MouseButton.Right])
            {
                if (_world.State == GameState.Game)
                    World.Player.Shoot(mousePos);
                
            }
        }
        

        private void ChangeLevel()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Level files|*.xml";
            dialog.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "levels");
            if (dialog.ShowDialog() == DialogResult.OK)
                _world.ChangeLevel(dialog.FileName);
            else
                _world.ChangeLevel("default.xml");
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _world.Init();

            _world.ChangeLevel("arena.xml");
            //ChangeLevel();
            //GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            //_sound.LoadSound("western", "western.mp3");
            
            System.Threading.Thread.Sleep(500);
            //_sound.PlaySound("western");
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _world.Camera.SetScreenDimensions(ClientRectangle.Width, ClientRectangle.Height);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            Vector2 mousePos = World.Camera.ToTileCoordinates(Mouse.X, Mouse.Y);
            World.Player.LookAt(mousePos);
            if (Mouse[MouseButton.Left])
            {
                if(_world.State == GameState.Game)
                    World.Player.MoveTowards(mousePos);
                else if (_world.State == GameState.Dialog)
                {
                    if (_world.GraphicsEngine.Dialog.AbortRectangle.Contains(Mouse.X, Mouse.Y))
                        _world.GraphicsEngine.Dialog.Abort();
                }
            }
            if (Keyboard[Key.Escape])
                Exit();
            if (Keyboard[Key.Left] || Keyboard[Key.A])
                _world.Player.Strafe(MoveDirection.Left);
            if (Keyboard[Key.Right] || Keyboard[Key.D])
                _world.Player.Strafe(MoveDirection.Right);
            if (e.Time > 0.15)
                _world.Update(0.15);
            else
                _world.Update(e.Time);
            _world.Camera.LookAt(_world.Player.Position);
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            _world.Draw(e.Time);
            SwapBuffers();
        }

        [STAThread]
        static void Main()
        {
            using (Game game = new Game())
            {
                //try
                //{
                    game.Run(50, 30);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex);
                //    Console.ReadKey();
                //}
            }
        }
    }
}