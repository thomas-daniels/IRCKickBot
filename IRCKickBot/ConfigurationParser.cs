using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace IRCKickBot
{
    class ConfigurationParser
    {
        public static Configuration Load(string filename, string configName)
        {
            XDocument doc = XDocument.Load(filename);
            Configuration cfg = null;
            if (doc.Root.Name != "Configurations")
            {
                throw new XmlException("Root name is not 'Configurations'.");
            }
            IEnumerable<XElement> elements = doc.Root.Elements();
            foreach (XElement elem in elements)
            {
                if (elem.Name != "Configuration")
                {
                    throw new XmlException("Invalid child element in Configurations: " + elem.Name);
                }
                IEnumerable<XElement> nameElements = elem.Elements("Name");
                if (!nameElements.Any())
                {
                    throw new XmlException("One of the Configuration elements does not have the required Name child element.");
                }
                else if (nameElements.Count() > 1)
                {
                    throw new XmlException("A Configuration element can only have one Name child element.");
                }
                string name = nameElements.Single().Value;
                if (name != configName)
                {
                    continue;
                }
                IEnumerable<XElement> patternElements = elem.Elements("Pattern");
                if (!patternElements.Any())
                {
                    throw new XmlException("One of the Configuration elements does not have the required Pattern child element. Please provide at least one.");
                }
                List<KickPattern> kickPatterns = new List<KickPattern>();
                foreach (XElement patternElem in patternElements)
                {
                    IEnumerable<XAttribute> reasonAttributes = patternElem.Attributes(XName.Get("Reason"));
                    if (!reasonAttributes.Any())
                    {
                        throw new XmlException("One of the Pattern elements does not have the required Reason attribute.");
                    }
                    string reason = reasonAttributes.First().Value;
                    kickPatterns.Add(new KickPattern(reason, patternElem.Value));
                }
                cfg = new Configuration(name, kickPatterns);
            }
            return cfg;
        }
    }
}
