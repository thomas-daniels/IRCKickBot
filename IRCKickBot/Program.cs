using System;
using System.Threading;

namespace IRCKickBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Host: ");
            string host = Console.ReadLine();
            Console.WriteLine("Port: ");
            int port = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Password: ");
            string password = Console.ReadLine();
            Console.WriteLine("Channel(s): ");
            string channels = Console.ReadLine();
            Console.WriteLine("Path to config: ");
            string configPath = Console.ReadLine();
            Console.WriteLine("Configuration name: ");
            string configName = Console.ReadLine();
            Console.Clear();
            Configuration conf = ConfigurationParser.Load(configPath, configName);
            Console.WriteLine("Configuration loaded.");
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
