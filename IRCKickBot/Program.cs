using System;
using System.Globalization;
using System.Threading;
using IRCKickBot.Properties;

namespace IRCKickBot
{
    class Program
    {
        static void Main(string[] args)
        {
            string configPath = null;
            string configName = null;
            if (args.Length > 0)
            {
                configPath = args[0];
                Console.WriteLine(Resources.pathToConfig + configPath);
            }
            if (args.Length > 1)
            {
                configName = args[1];
                Console.WriteLine(Resources.configurationName + configName);
            }
            if (configPath == null)
            {
                Console.WriteLine(Resources.pathToConfig);
                configPath = Console.ReadLine();
            }
            if (configName == null)
            {
                Console.WriteLine(Resources.configurationName);
                configName = Console.ReadLine();
            }
            Configuration conf = ConfigurationParser.Load(configPath, configName);
            if (conf == null)
            {
                Console.WriteLine(Resources.configNotFound);
                return;
            }
            Console.WriteLine(Resources.configLoaded);
            if (conf.Host == null)
            {
                Console.WriteLine(Resources.host);
                conf.Host = Console.ReadLine();
            }
            if (!conf.Port.HasValue)
            {
                Console.WriteLine(Resources.port);
                conf.Port = Int32.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            }
            if (conf.Nickname == null)
            {
                Console.WriteLine(Resources.nickname);
                conf.Nickname = Console.ReadLine();
            }
            if (conf.Password == null)
            {
                Console.WriteLine(Resources.password);
                conf.Password = Console.ReadLine();
            }
            if (conf.Channels == null)
            {
                Console.WriteLine(Resources.channels);
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
                    Console.WriteLine(Resources.ctrlDClosing);
                    client.Close();
                    break;
                }
            }
        }
    }
}
