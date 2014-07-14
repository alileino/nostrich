using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class QuestManager
    {
        Dictionary<string, Quest> _quests = new Dictionary<string,Quest>();
        Queue<Quest> _questCreationQueue = new Queue<Quest>();
        Queue<Quest> _questDeletionQueue = new Queue<Quest>();
        Queue<Trigger> _triggerCreationQueue = new Queue<Trigger>();
        Queue<Trigger> _triggerDeletionQueue = new Queue<Trigger>();
        List<Trigger> _triggers = new List<Trigger>();
        PawnCreator _pawnCreator;

        public QuestManager(PawnCreator pawnCreator)
        {
            _pawnCreator = pawnCreator;
            ScriptBindings.ScriptHasEventRun += ScriptBindings_ScriptHasEventRun;
            ScriptBindings.ScriptSaveEvent += SaveManager.SaveEvent;
        }

        bool ScriptBindings_ScriptHasEventRun( string quest, string gameEvent)
        {
            return (_quests[quest].Events[gameEvent].Started);
        }

        public void Load()
        {
            Dictionary<string, Quest> newQuests = QuestLoader.LoadAll();
            foreach (Quest q in newQuests.Values)
                _questCreationQueue.Enqueue(q);
        }

        public void Unload()
        {
            foreach (Quest q in _quests.Values)
                _questDeletionQueue.Enqueue(q);
        }

        private void QuestTriggered(Trigger trigger, out bool cancel)
        {
            
             bool eventRan = _quests[trigger.Quest].FireEvent(trigger.Event, trigger.Parameters);
            if (!eventRan)
            {
                cancel = true;
                return;
            }
            else
            {
               // if(trigger.Quest != "global" && _quests[trigger.Quest].Events[trigger.Event].AutoSave)
               //     SaveManager.SaveEvent(trigger.Quest,trigger.Event);
            }
            if (trigger.Completed)
                UnloadTrigger(trigger);
            cancel = false;
        }


        public void Update(double ticksPassed)
        {
            foreach (Quest quest in _quests.Values)
                quest.Update(ticksPassed);

            while(_questDeletionQueue.Count > 0)
            {
                Quest q = _questDeletionQueue.Dequeue();
                _quests.Remove(q.Name);
            }
            while (_questCreationQueue.Count > 0)
            {
                Quest q = _questCreationQueue.Dequeue();
                _quests.Add(q.Name, q);
                q.Init();
            }
            while (_triggerDeletionQueue.Count > 0)
            {
                Trigger t = _triggerDeletionQueue.Dequeue();
                _triggers.Remove(t);
            }
            while (_triggerCreationQueue.Count > 0)
            {
                Trigger t = _triggerCreationQueue.Dequeue();
                _triggers.Add(t);
            }

        }

        public void LoadTriggers(List<Trigger> triggers)
        {
            
            foreach (Trigger trigger in triggers)
            {
                LoadTrigger(trigger);
            }
        }

        private void LoadTrigger(Trigger trigger)
        {
            if (trigger is AreaTrigger)
            {
                AreaTrigger areaTrigger = (AreaTrigger)trigger;
                _pawnCreator.PlayerChangedTile += areaTrigger.TryLaunch;
            }
            trigger.Triggered += QuestTriggered;
            _triggerCreationQueue.Enqueue(trigger);
        }

        private void UnloadTrigger(Trigger trigger)
        {
            trigger.Triggered -= QuestTriggered;
            if (trigger is AreaTrigger)
            {
                AreaTrigger areaTrigger = (AreaTrigger)trigger;
                _pawnCreator.PlayerChangedTile -= areaTrigger.TryLaunch;
            }
            _triggerDeletionQueue.Enqueue(trigger);

        }

        internal void UnloadTriggers(List<Trigger> triggers)
        {
            foreach (Trigger trigger in triggers)
                UnloadTrigger(trigger);
        }

        internal void UnloadTriggers()
        {
            UnloadTriggers(_triggers);
        }

        internal void LoadEvents(List<SaveManager.RanEvent> ranEvents)
        {
            foreach (var ranEvent in ranEvents)
                _quests[ranEvent.Quest].Events[ranEvent.Event].Started = true;
        }
    }
}
