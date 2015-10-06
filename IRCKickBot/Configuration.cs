using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IRCKickBot
{
    class Configuration
    {
        public string Name { get; private set; }
        public ReadOnlyCollection<KickPattern> Patterns { get; private set; }
        public Configuration(string name, IEnumerable<KickPattern> patterns)
        {
            Name = name;
            Patterns = new ReadOnlyCollection<KickPattern>(patterns.ToList());
        }
    }
}
