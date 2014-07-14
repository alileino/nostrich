using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WesternLib
{
    public class GraphicsEngine
    {
        Player _player;
        Vector3 _healthLoc = new Vector3(-5, 5, 0);
        Vector3 _weaponLoc;
        SpriteCollection _hud;
        ResourceManager _resourceMgr;
        Camera _camera;
        DialogUI _dialogUI;
        TextManager _textMgr;
        public DialogUI Dialog {  get { return _dialogUI; } }

        public GraphicsEngine(ResourceManager resourceManager, Camera camera, Player player) // todo: passing the player is really bad design;
        {
            _player = player;
            _resourceMgr = resourceManager;
            _camera = camera;
            _hud = _resourceMgr.Sprites["hud"];
            _healthLoc = new Vector3(40, 50, 0);
            _weaponLoc = new Vector3(100, 50, 0);
            Init();
        }

        public void Init()
        {
            _textMgr = new TextManager();
            _textMgr.Init();
            _dialogUI = new DialogUI(_textMgr, _resourceMgr, _camera);
        }

        internal void Draw(GameState state, double ticks)
        {
            GL.PushMatrix();
            //GL.LoadIdentity();
            GL.Translate(0, 0, -99.99f);
            GL.Color4(1f, 1f, 1f, 1f);
            
            if (state == GameState.Game)
            {
                DrawHealth();
                DrawWeapons();
            }
            else
            {
                _dialogUI.Draw(ticks);
            }
            GL.PopMatrix();
        }

        private void DrawHealth()
        {

            int healthTicks = (int)((_player.Health+0.1) * 10);
            _hud["health_bottle"].DrawAbsolute(_healthLoc);
            if(healthTicks >= 1)
                _hud["health_bottom"].DrawAbsolute(_healthLoc);
            for (int i = 2; i <= 9; i++)
            {
                if (healthTicks >= i)
                    _hud["health_middle_" + (i - 1)].DrawAbsolute(_healthLoc);
                else
                    break;
            }
            if (healthTicks >= 10)
                _hud["health_top"].DrawAbsolute(_healthLoc);

        }

        private void DrawWeapons()
        {
            DrawWeapon(_player.Equipped.Right, _weaponLoc);
            DrawWeapon(_player.Equipped.Left, _weaponLoc + new Vector3(50, 0f, 0f));
            //_hud["revolver_6"].Draw(_weaponLoc, 0);
        }

        private void DrawWeapon(Weapon weapon, Vector3 _weaponLoc)
        {
            if (weapon.Reloading)
                GL.Color4(1f, 1f, 1f, 0.5f);
            else
                GL.Color4(1f, 1f, 1f, 1f);
            _hud["revolver_" + weapon.BulletsLoaded].DrawAbsolute(_weaponLoc);
        }
    }

}
