using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace IRCKickBot
{
    static class ConfigurationParser
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
                IEnumerable<XElement> pluginElements = elem.Elements("Plugin");
                if (!patternElements.Any() && !pluginElements.Any())
                {
                    throw new XmlException("One of the Configuration elements does not have any Pattern or Plugin child element. Please provide at least one.");
                }
                List<KickPattern> kickPatterns = new List<KickPattern>();
                List<Plugin> plugins = new List<Plugin>();
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
                foreach (XElement pluginElem in pluginElements)
                {
                    IEnumerable<XElement> methodElements = pluginElem.Elements("Method");
                    if (!methodElements.Any())
                    {
                        throw new XmlException("One of the Plugin elements does not have the required Method element. Please provide at least one.");
                    }
                    XAttribute assemblyPathAttribute = pluginElem.Attribute(XName.Get("AssemblyPath"));
                    if (assemblyPathAttribute == null)
                    {
                        throw new XmlException("The Plugin element must have an AssemblyPath attribute.");
                    }
                    Plugin p = new Plugin(assemblyPathAttribute.Value);
                    foreach (XElement methodElem in methodElements)
                    {
                        string typeAndMethod = methodElem.Value;
                        string[] parts = typeAndMethod.Split('.');
                        if (parts.Length == 1)
                        {
                            throw new XmlException("The Method element must contain the full method name, in a format like `<full type name>.<method name>` (where >full type name> = <optional namespaces>.<type name>)");
                        }
                        string methodName = parts.Last();
                        string typeName = string.Join(".", parts.Take(parts.Length - 1));
                        p.LoadMethod(typeName, methodName);
                    }
                    plugins.Add(p);
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
                cfg = new Configuration(kickPatterns, plugins, host, port, username, password, channels);
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
