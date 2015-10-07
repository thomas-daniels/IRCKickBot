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
            if (conf.Host == null)
            {
                Console.WriteLine("Host: ");
                conf.Host = Console.ReadLine();
            }
            if (!conf.Port.HasValue)
            {
                Console.WriteLine("Port: ");
                conf.Port = Int32.Parse(Console.ReadLine());
            }
            if (conf.Username == null)
            {
                Console.WriteLine("Username: ");
                conf.Username = Console.ReadLine();
            }
            if (conf.Password == null)
            {
                Console.WriteLine("Password: ");
                conf.Password = Console.ReadLine();
            }
            if (conf.Channels == null)
            {
                Console.WriteLine("Channel(s): ");
                conf.Channels = Console.ReadLine();
            }
            Console.Clear();
            IrcClient client = new IrcClient(conf);
            client.ConnectAndJoin();
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
