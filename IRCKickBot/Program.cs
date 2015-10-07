using System;
using System.Threading;

namespace IRCKickBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Path to config: ");
            string configPath = Console.ReadLine();
            Console.WriteLine("Configuration name: ");
            string configName = Console.ReadLine();
            Configuration conf = ConfigurationParser.Load(configPath, configName);
            if (conf == null)
            {
                Console.WriteLine("No such configuration found.");
                return;
            }
            Console.WriteLine("Configuration loaded.");
            string host = conf.Host;
            if (host == null)
            {
                Console.WriteLine("Host: ");
                host = Console.ReadLine();
            }
            int port = -1;
            if (!conf.Port.HasValue)
            {
                Console.WriteLine("Port: ");
                port = Int32.Parse(Console.ReadLine());
            }
            else
            {
                port = conf.Port.Value;
            }
            string username = conf.Username;
            if (username == null)
            {
                Console.WriteLine("Username: ");
                username = Console.ReadLine();
            }
            string password = conf.Password;
            if (password == null)
            {
                Console.WriteLine("Password: ");
                password = Console.ReadLine();
            }
            string channels = conf.Channels;
            if (channels == null)
            {
                Console.WriteLine("Channel(s): ");
                channels = Console.ReadLine();
            }
            Console.Clear();
            IrcClient client = new IrcClient(host, port, conf);
            client.Connect(username, username, password);
            Thread.Sleep(5000);
            client.Send("MODE " + username + " -i");
            client.Join(channels);
            Thread loopThr = new Thread(() =>
            {
                client.ReceiveLoop();
            });
            loopThr.Start();
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Modifiers == ConsoleModifiers.Control && cki.Key == ConsoleKey.D)
                {
                    Console.WriteLine("Ctrl+D received, closing...");
                    client.Close();
                    break;
                }
            }
        }
    }
}
