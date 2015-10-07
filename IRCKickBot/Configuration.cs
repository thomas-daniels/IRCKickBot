using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IRCKickBot
{
    class Configuration
    {
        public string Name { get; private set; }
        public ReadOnlyCollection<KickPattern> Patterns { get; private set; }
        public string Host { get; private set; }
        public int? Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Channels { get; private set; }
        public Configuration(string name, IEnumerable<KickPattern> patterns, string host, int? port, string username, string password, string channels)
        {
            Name = name;
            Patterns = new ReadOnlyCollection<KickPattern>(patterns.ToList());
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Channels = channels;
        }
    }
}
