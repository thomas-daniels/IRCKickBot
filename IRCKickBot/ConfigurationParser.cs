using System.Collections.Generic;
using System.Globalization;
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
                string name = LoadSingle(elem, "Name");
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
                string host = LoadNoneOrSingle(elem, "Host");
                string portStr = LoadNoneOrSingle(elem, "Port");
                int? port = new int?();
                if (portStr != null)
                {
                    int portI;
                    if (int.TryParse(portStr, out portI))
                    {
                        port = portI;
                    }
                    else
                    {
                        throw new XmlException("The 'Port' element must contain a number.");
                    }
                }
                string username = LoadNoneOrSingle(elem, "Username");
                string password = LoadNoneOrSingle(elem, "Password");
                string channels = LoadNoneOrSingle(elem, "Channels");
                cfg = new Configuration(kickPatterns, host, port, username, password, channels);
            }
            return cfg;
        }

        private static string LoadSingle(XElement elem, string childName)
        {
            IEnumerable<XElement> children = elem.Elements(XName.Get(childName));
            if (children.Count() != 1)
            {
                throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Element '{0}' must have exactly one '{1}' child element.", elem.Name, childName));
            }
            return children.Single().Value;
        }

        private static string LoadNoneOrSingle(XElement elem, string childName)
        {
            IEnumerable<XElement> children = elem.Elements(XName.Get(childName));
            if (children.Count() > 1)
            {
                throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Element '{0}' cannot have more than one '{1}' as children.", elem.Name, childName));
            }
            if (!children.Any())
            {
                return null;
            }
            XElement child = children.Single();
            return child.Value;
        }
    }
}
