using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WesternLib
{
    public static class SaveManager
    {
        public class RanEvent
        {
            public string Quest;
            public string Event;
            public RanEvent(string quest, string gameEvent)
            {
                Quest = quest;
                Event = gameEvent;
            }
        }

        static List<RanEvent> _ranEvents = new List<RanEvent>();

        public static List<RanEvent> RanEvents { get { return _ranEvents; } }
        static Point _spawnPoint;

        public static Point SpawnPoint { get { return _spawnPoint; } }

        public static void SaveEvent(string quest, string gameEvent)
        {
            _ranEvents.Add(new RanEvent(quest, gameEvent));
        }

        public static void SaveSpawnPoint(Point position)
        {
            _spawnPoint = position;
        }

        public static void Save()
        {
            using (XmlWriter writer = getXmlWriter(Path.Combine("data", "save.xml")))
            {
                writer.WriteStartElement("Save");
                writer.WriteStartElement("Player");
                writer.WriteAttribute("X", _spawnPoint.X);
                writer.WriteAttribute("Y", _spawnPoint.Y);
                writer.WriteEndElement();
                foreach (RanEvent ranEvent in _ranEvents)
                {
                    writer.WriteStartElement("RanEvent");
                    writer.WriteAttribute("Quest", ranEvent.Quest);
                    writer.WriteAttribute("Event", ranEvent.Event);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        public static void Load()
        {
            _ranEvents.Clear();
            try
            {

                XDocument doc = XDocument.Load(Path.Combine("data", "save.xml"));
                XElement playerElement = doc.Root.Element("Player");
                int x = playerElement.GetAttribute("X", 0);
                int y = playerElement.GetAttribute("Y", 0);
                _spawnPoint.X = x;
                _spawnPoint.Y = y;
                foreach (XElement eventElement in doc.Root.Elements("RanEvent"))
                {
                    RanEvent ranEvent = new RanEvent(eventElement.GetAttribute("Quest"), eventElement.GetAttribute("Event"));
                    _ranEvents.Add(ranEvent);
                }
            }
            catch (Exception)
            {
            }
        }
        
        private static XmlWriter getXmlWriter(string filename)
        {
            return XmlWriter.Create(filename, getXmlWriterSettings());
        }

        private static XmlWriterSettings getXmlWriterSettings()
        {
            var settings = new XmlWriterSettings();
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.Indent = true;
            return settings;
        }
    }
}
