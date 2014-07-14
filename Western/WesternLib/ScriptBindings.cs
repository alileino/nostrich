using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class ScriptBindings
    {
        public delegate Pawn SpawnHandler(string type, double xpos, double ypos,string name);
        internal static event SpawnHandler ScriptSpawn;
        public delegate void ChangeLevelHandler(string level, string location);
        internal static event ChangeLevelHandler ScriptChangeLevel;
        public delegate Dialog ShowDialogHandler(string filename);
        internal static event ShowDialogHandler ScriptShowDialog;
        public delegate void PlaySoundHandler(string name);
        internal static event PlaySoundHandler ScriptPlaySound;
        public delegate void SaveLoadHandler();
        internal static event SaveLoadHandler ScriptSave;
        internal static event SaveLoadHandler ScriptLoad;
        public delegate bool EventRanHandler(string quest, string gameEvent);
        public static event EventRanHandler ScriptHasEventRun;
        public delegate void SaveEventHandler(string quest, string gameEvent);
        public static event SaveEventHandler ScriptSaveEvent;

        public delegate string CurrentLevelHandler();
        public static event CurrentLevelHandler ScriptCurrentLevel;

        public static Player Player;

        public Pawn Spawn(string type, double xpos = 100.0, double ypos = 100.0, string name = "noname")
        {
            if (ScriptSpawn != null)
                return ScriptSpawn(type, xpos, ypos,name);
            return null;    
        }

        public void Save()
        {
            if (ScriptSave != null)
                ScriptSave();
        }

        public string CurrentLevel()
        {
            if (ScriptCurrentLevel != null)
                return ScriptCurrentLevel();
            return "";
        }
        public void Load()
        {
            if (ScriptLoad != null)
                ScriptLoad();
        }

        public void SaveEvent(string quest, string gameEvent)
        {
            if (ScriptSaveEvent != null)
                ScriptSaveEvent(quest, gameEvent);
        }

        public bool HasEventRun(string quest, string gameEvent)
        {
            if (ScriptHasEventRun != null)
            {
                return ScriptHasEventRun(quest, gameEvent);

            }
            Console.WriteLine("Warning: ScriptBindings.HasEventRun is not registered: it always returns true.");
            return true;
        }

        public void PlaySound(string name)
        {
            if (ScriptPlaySound != null)
                ScriptPlaySound(name);
        }

        public void ChangeLevel(string level, string location="default")
        {
            if (ScriptChangeLevel != null)
                ScriptChangeLevel(level, location);
        }

        public Dialog ShowDialog(string filename)
        {
            if (ScriptShowDialog != null)
                return ScriptShowDialog(filename);
            return null;
        }


    }
}
