using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IRCKickBot
{
    class Configuration
    {
        public ReadOnlyCollection<KickPattern> Patterns { get; set; }
        public ReadOnlyCollection<Plugin> Plugins { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Channels { get; set; }
        public Configuration(IEnumerable<KickPattern> patterns, IEnumerable<Plugin> plugins, string host, int? port, string nickname, string password, string channels)
        {
            Patterns = new ReadOnlyCollection<KickPattern>(patterns.ToList());
            Plugins = new ReadOnlyCollection<Plugin>(plugins.ToList());
            Host = host;
            Port = port;
            Nickname = nickname;
            Password = password;
            Channels = channels;
        }
    }
}
