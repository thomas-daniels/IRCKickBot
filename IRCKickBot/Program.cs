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
            Console.Clear();
            IrcClient client = new IrcClient(host, port);
            client.Connect(username, username, password);
            System.Threading.Thread.Sleep(5000);
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
