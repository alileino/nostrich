
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WesternLib
{
    public class QuestLoader : XmlHelper
    {
        static readonly string QUEST_DIRECTORY = "Quests";
        static int _actionCounter = 1000;
        public static Dictionary<string, Quest> LoadAll()
        {

                DirectoryInfo questDir = new DirectoryInfo(QUEST_DIRECTORY);
                Dictionary<string, Quest> quests = new Dictionary<string, Quest>();
                foreach (FileInfo file in questDir.EnumerateFiles())
                {
                    try
                    {
                        XDocument doc = XDocument.Load(file.FullName);
                        Quest quest = new Quest();
                        quest.Name = doc.Root.GetAttribute("Name");
                        quest.Map = doc.Root.GetAttribute("Map");
                        foreach (XElement eventElement in doc.Root.Elements("Event"))
                        {
                            GameEvent ge = LoadEvent(eventElement);
                            quest.Events.Add(ge.Name, ge);
                        }
                        quests.Add(quest.Name, quest);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Quest " + file.Name + " could not be loaded.");
                    }
                
                

                
                }
            return quests;
        }

        private static GameEvent LoadEvent(XElement eventElement)
        {
            GameEvent gameEvent = new GameEvent(eventElement.GetAttribute("Name"));
            gameEvent.After = eventElement.GetAttribute("After", null);
            gameEvent.AutoSave = eventElement.GetAttribute("AutoSave", true);

            
            foreach (XElement actionElement in eventElement.Elements())
            {
                string name = actionElement.GetAttribute("Name", "Action_" + _actionCounter.ToString());
                _actionCounter++;
                string type = actionElement.Name.LocalName;
                string content = actionElement.Value;
                EventAction action;
                switch (type)
                {
                    case "Script":
                        action = new ScriptAction(content);
                        break;
                    default:
                        Console.WriteLine("EventAction: " + type + " is not implemented.");
                        continue;
                }
                action.Name = name;
                gameEvent.AddAction(action);

            }
            return gameEvent;
        }
    }
}
