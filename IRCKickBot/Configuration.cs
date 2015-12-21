using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IRCKickBot
{
    class Configuration
    {
        public ReadOnlyCollection<KickPattern> Patterns { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Channels { get; set; }
        public Configuration(IEnumerable<KickPattern> patterns, string host, int? port, string username, string password, string channels)
        {
            Patterns = new ReadOnlyCollection<KickPattern>(patterns.ToList());
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Channels = channels;
        }
    }
}
