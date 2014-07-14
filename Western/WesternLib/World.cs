using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public enum GameState
    {
        Game,
        Dialog,
        Editor,
        Loading
    };

    public class World
    {
        Level _testLevel;
        ResourceManager _resourceMgr;
        Camera _camera;
        PawnCreator _pawnCreator;
        QuestManager _questMgr;
        RenderingOptions _renderingOptions = new RenderingOptions();
        GraphicsEngine _graphicsEngine;
        GameState _state;
        
        
        public Camera Camera { get { return _camera; } }
        public ResourceManager ResourceManager { get { return _resourceMgr; } }
        public GraphicsEngine GraphicsEngine { get { return _graphicsEngine; } }
        public Level CurrentLevel { get { return _testLevel; } }
        public Player Player { get { return _pawnCreator.Player; } }
        public RenderingOptions Options { get { return _renderingOptions; } }
        public GameState State { get { return _state; } set { _state = value; } }

        public World()
        {
            _camera = new Camera();
            Level.RenderingOptions = Options;
            ScriptBindings.ScriptChangeLevel += ChangeLevel;
            ScriptBindings.ScriptShowDialog += ShowDialog;
            ScriptBindings.ScriptSave += Save;
            ScriptBindings.ScriptLoad += ScriptLoad;
            ScriptBindings.ScriptCurrentLevel += GetCurrentLevel;
        }

        private string GetCurrentLevel()
        {
            return _testLevel.Name;
        }



        void Dialog_DialogEnded(Dialog dialog)
        {
            if (dialog == _graphicsEngine.Dialog.Dialog)
                _state = GameState.Game;
        }

        public void Draw3D(double deltaT)
        {
            double ticks = deltaT * GameConstants.TICKS_PER_SECOND;
            if (_state == GameState.Game || _state == GameState.Editor)
            {
                _camera.Ready3D();
                _camera.Translate();
                _testLevel.Draw(Camera.VisibleRectangle, ticks);
            }
        }


        public void Draw2D(double deltaT)
        {
            double ticks = deltaT * GameConstants.TICKS_PER_SECOND;
            _camera.Ready2D();
            _graphicsEngine.Draw(_state, ticks);
        }
        public void Draw(double deltaT)
        {
            Draw3D(deltaT);
            Draw2D(deltaT);
        }

        public void Update(double deltaT)
        {
            double ticks = deltaT * GameConstants.TICKS_PER_SECOND;
            if (_state == GameState.Game)
            {
                _testLevel.Update(ticks);
                _questMgr.Update(ticks);
            }
            else if (_state == GameState.Loading)
            {
                Load();
                _state = GameState.Game;
            }
        }

        public void Init()
        {
            _resourceMgr = new ResourceManager();
            _resourceMgr.LoadResources();

            _pawnCreator = new PawnCreator(_resourceMgr);

            ScriptBindings.Player = _pawnCreator.Player;
            _pawnCreator.Player.PlayerRespawning += PlayerRespawning;
            _questMgr = new QuestManager(_pawnCreator);
            _graphicsEngine = new GraphicsEngine(_resourceMgr, _camera, _pawnCreator.Player);
            _graphicsEngine.Dialog.DialogEnded += Dialog_DialogEnded;
            Camera.LookAt(Player.Position);
        }

        private void PlayerRespawning(Pawn sender)
        {
            (sender as Player).Health = 1f;
            Load();

        }

        public void ChangeLevel(string fileName, string location="default")
        {
            _questMgr.Unload();
            _questMgr.UnloadTriggers();
            _questMgr.Load();
            if (CurrentLevel != null)
            {
                CurrentLevel.Unload();
                _questMgr.UnloadTriggers(_testLevel.Triggers);
                if(location=="default" || location=="")
                    location = "from:" + CurrentLevel.Name;
            }
            _pawnCreator.RemovePawns();
            _testLevel = new Level(_resourceMgr, _pawnCreator);
            
            if (fileName != null)
                _testLevel.Load(fileName);
            else
                _testLevel.New();
            
            _questMgr.LoadTriggers(_testLevel.Triggers);
            _questMgr.LoadEvents(SaveManager.RanEvents);
            _pawnCreator.Player.Position = _testLevel.GetLocationOrDefault(location).ToVector2() ;
            SaveManager.SaveSpawnPoint(_testLevel.GetLocationOrDefault(location));  
        }

        public Dialog ShowDialog(string filename)
        {
            Player.Stop();
            _state = GameState.Dialog;
            _graphicsEngine.Dialog.CreateFromFile(filename);
            return _graphicsEngine.Dialog.Dialog;
        }

        public void Save()
        {
            Console.WriteLine("Saving Game");
            SaveManager.SaveSpawnPoint(Player.Position.ToPoint());
            SaveManager.Save();
            
        }

        public void Load()
        {
            Console.WriteLine("Loading game");
            SaveManager.Load();
            ChangeLevel(_testLevel.Name);
            Player.Position = SaveManager.SpawnPoint.ToVector2();
        }

        private void ScriptLoad()
        {
            _state = GameState.Loading;
        }

    }


}
