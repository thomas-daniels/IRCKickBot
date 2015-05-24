using System;
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
        public IrcClient(string host, int port)
        {
            Host = host;
            Port = port;
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

        public void Join(string channel)
        {
            Send("JOIN " + channel);
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
                    if (Regex.IsMatch(line, "\\bfuck you\\b|asshole|co+ck ?su+cker"))
                    {
                        Send("KICK " + parts[2] + " " + user + " :Offensive posts.");
                    }
                }
            }
        }

        public void Close()
        {
            _client.Close();
        }
    }
}
