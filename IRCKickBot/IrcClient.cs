using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

namespace IRCKickBot
{
    class IrcClient
    {
        public string Host { get; }
        public int Port { get; }

        TcpClient _client = new TcpClient();
        NetworkStream _stream;
        bool shouldClose = false;
        Regex _offensive_regex;
        HashSet<string> _joinedChannels = new HashSet<string>();

        public IrcClient(string host, int port)
        {
            Host = host;
            Port = port;
            _offensive_regex = new Regex("\\bfuck you\\b|asshole|co+ck ?su+cker", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public void Connect(string nickname, string username, string password)
        {
            _client.Connect(Host, Port);
            _stream = _client.GetStream();
            if (!String.IsNullOrEmpty(password))
            {
                Send("PASS " + password);
            }
            Send("NICK " + nickname);
            Send("USER " + username + " 0 * " + username);
        }

        public void Join(string channels)
        {
            string[] channelList = channels.Split(',');
            foreach (string c in channelList)
            {
                _joinedChannels.Add(c.Trim());
            }
            Send("JOIN " + channels);
        }

        public void Send(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message + "\r\n");
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void ReceiveLoop()
        {
            using (StreamReader sr = new StreamReader(_stream))
            {
                string line;
                try
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        if (line.StartsWith("PING") && line[4] == ' ')
                        {
                            string server = line.Split(' ')[1];
                            Send("PONG " + server);
                        }
                        if (!line.StartsWith(":"))
                            continue;
                        string[] parts = line.Remove(0, 1).Split(' ');
                        string user = parts[0].Split('!')[0];
                        if (_joinedChannels.Contains(parts[2]) && _offensive_regex.IsMatch(line))
                        {
                            Send("KICK " + parts[2] + " " + user + " :Offensive posts.");
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    if (!shouldClose)
                    {
                        throw;
                    }
                }
                catch (IOException)
                {
                    if (!shouldClose)
                    {
                        throw;
                    }
                }
            }
        }

        public void Close()
        {
            shouldClose = true;
            Send("QUIT :Owner requested quit");
            _client.Close();
        }
    }
}
