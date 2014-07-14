#region Scripting
    using IronPython.Hosting;
    using Microsoft.Scripting;
    using Microsoft.Scripting.Hosting;
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public struct GameEventRunArgs
    {
        public GameEvent Event { get; private set;}
        public Dictionary<string,string> Parameters {get; private set;}

        public GameEventRunArgs(GameEvent gameEvent, Dictionary<string, string> parameters)
            :this()
        {
            Event = gameEvent;
            Parameters = parameters;
        }

        public static implicit operator GameEvent(GameEventRunArgs runArgs)
        {
            return runArgs.Event;
        }

    }

    public class Quest : Scriptable
    {
        private bool _started = false;
        private bool _initialized = false;
        string _name;
        private Dictionary<string, GameEvent> _events = new Dictionary<string, GameEvent>();
        private List<GameEvent> _runningEvents = new List<GameEvent>();
        private List<GameEvent> _stoppedEvents = new List<GameEvent>();

        public string Name { get { return _name; } internal set { _name = value; } }
        public string Map { get; set; }
        public Dictionary<string, GameEvent> Events { get { return _events; } }
        public bool Started { get { return _started; } }
        public bool Initialized { get { return _initialized; } }

        public void Init()
        {
            if (_events.ContainsKey("init"))
            {
                FireEvent("init");
            }

            foreach (GameEvent e in _events.Values)
            {
                foreach (EventAction action in e.Actions)
                {
                    ScriptAction script = action as ScriptAction;
                    if (script != null)
                        AddScript(script.Name, script.Source);
                }
            }
            _initialized = true;
        }

        public void Update(double ticksPassed)
        {
            foreach (GameEvent e in _runningEvents)
                ProcessEvent(e, ticksPassed);
            foreach (GameEvent e in _stoppedEvents)
                _runningEvents.Remove(e);
            _stoppedEvents.Clear();
        }

        public bool FireEvent(string name, Dictionary<string, string> parameters=null)
        {
            GameEvent e = _events[name];
            if (_name != "global" && !PrerequisiteMet(e))
                return false;
            if (e.Name == "start")
                _started = true;
            e.Stopped += EventStopped;
            e.Parameters = parameters;
            e.Start();
            _runningEvents.Add(e);
            return true;
        }

        public bool PrerequisiteMet(GameEvent e)
        {
            return ( (e.After == null || e.After == "" || _events[e.After].Started));
        }

        private void ProcessEvent(GameEvent e, double ticksPassed)
        {
            EventAction action = e.CurrentAction;
            
            if (action is ScriptAction)
            {
                Execute(action.Name, e.Parameters);
            }
            action.Update(ticksPassed);
            
        }

        private void EventStopped(GameEvent e)
        {
            e.Stopped -= EventStopped;
            _stoppedEvents.Add(e);
        }
    }


    public class GameEvent
    {
        private List<EventAction> _actions = new List<EventAction>();
        string _name;
        private int _curActionIndex = 0;
        string _after;
        public Dictionary<string, string> Parameters { get; set; }
        public string Name { get { return _name; } internal set { _name = value; } }
        public bool Started { get; set; }
        public string After { get { return _after; } set {  _after = value; } }
        public bool AutoSave { get; set; }
        public List<EventAction> Actions { get { return _actions; } }

        public EventAction CurrentAction { get { return _actions[_curActionIndex]; } }
        public delegate void StoppedEventHandler(GameEvent e);
        public event StoppedEventHandler Stopped;

        public GameEvent(string name)
        {
            _name = name;
        }

        public void AddAction(EventAction action)
        {
            _actions.Add(action);
            action.Stopped += ActionStopped;
        }

        public void Start()
        {
            Started = true;
            Console.WriteLine("Event: " + _name + " has started.");
            _curActionIndex = 0;
        }

        public void Stop()
        {
            if (Stopped != null)
                Stopped(this);
        }

        private void ActionStopped(EventAction action)
        {
            _curActionIndex++;
            if (_curActionIndex == _actions.Count)
                Stop();
        }

    }

    public abstract class EventAction
    {
        string _name;
        public string Name { get { return _name; } internal set { _name = value; } }
        public delegate void StopEventHandler(EventAction action);
        public event StopEventHandler Stopped;

        protected void Stop()
        {
            if (Stopped != null)
                Stopped(this);
        }

        public virtual void Update(double ticksPassed)
        {
            Stop();
        }
    }

    public class ScriptAction : EventAction
    {
        private string _source;

        internal string Source { get { return _source; } }

        public ScriptAction(string source)
        {
            _source = source;
        }
    }
}
