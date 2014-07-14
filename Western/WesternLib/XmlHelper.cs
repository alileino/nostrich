using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WesternLib
{
    public class XmlHelper
    {
        protected XmlWriter getXmlWriter(string filename)
        {
            return XmlWriter.Create(filename, getXmlWriterSettings());
        }

        protected XmlWriterSettings getXmlWriterSettings()
        {
            var settings = new XmlWriterSettings();
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.Indent = true;
            return settings;
        }
    }

    public static class XmlLinqExtensions
    {
        public static string GetAttribute(this XElement element, XName name)
        {
            return (string)element.Attribute(name);
        }

        public static int GetAttribute(this XElement element, XName name, int defaultValue)
        {
            return (int?)element.Attribute(name) ?? defaultValue;
        }

        public static bool GetAttribute(this XElement element, XName name, bool defaultValue)
        {
            return (bool?)element.Attribute(name) ?? defaultValue;
        }

        public static string GetAttribute(this XElement element, XName name, string defaultValue)
        {
            return (string)element.Attribute(name) ?? defaultValue;
        }

        public static float GetAttribute(this XElement element, XName name, float defaultValue)
        {
            string value = (string)element.Attribute(name);
            if (value == null)
                return defaultValue;
            else
                return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture); // specifying culture assures that parse always expects "." as a decimal separator
        }

        public static void WriteAttribute<T>(this XmlWriter writer, string attributeString, T value)
        {
            writer.WriteAttributeString(attributeString, value.ToString());
        }
    }
}
