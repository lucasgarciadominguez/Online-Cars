using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESNEServer23
{
    internal class Connection
    {
        static byte[] ibuffer = new byte[2048];
        static byte[] obuffer = new byte[2048];

        private TcpClient client;
        public Socket socket { get; private set; }

        public Action<string> SendToAllDelegate;
        public Action<string, Connection> SendToAllButThisDelegate;
        public int numberOfConnection;
        public bool startCountdown=false;
        public Connection(TcpClient client)
        {
            this.client = client;
            this.socket = client.Client;
        }

        public override string ToString()
        {
            if (socket.Connected)
            {
                return ((IPEndPoint)socket.LocalEndPoint).Address.ToString() + ":" + ((IPEndPoint)socket.LocalEndPoint).Port.ToString();
            }
            else
                return "Client disconnected.";
        }

        public bool Process()
        {
            if (socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0)
            {
                Console.WriteLine("Client " + this + " disconnected.");
                SendToAllButThisDelegate.Invoke("userDisconected", this);

                return false;
            }

            if (socket.Available > 0)
            {
                int bytesReceived = socket.Available;
                byte[] strBuffer = new byte[bytesReceived];
                try
                {
                    socket.Receive(ibuffer, bytesReceived, SocketFlags.None);

                }
                catch (ArgumentOutOfRangeException argumentOutOfRangeException)
                {
                    Console.Write($"Error: {argumentOutOfRangeException.Message}");
                }

                Buffer.BlockCopy(ibuffer, 0, strBuffer, 0, bytesReceived);
                string str = Encoding.ASCII.GetString(strBuffer);
                if (str.Contains("join"))
                {
                    string[] messages = str.Split('$');
                    messages[0] = messages[0] + "|" + numberOfConnection + '$';
                    Console.WriteLine(messages[0]);
                    SendToAllDelegate.Invoke(messages[0]);

                }
                else
                {
                    Console.WriteLine(str);

                    SendToAllButThisDelegate.Invoke(str, this);

                }

            }

            return true;
        }

        public void Send(string str)
        {
            byte[] strBuffer = Encoding.ASCII.GetBytes(str);
            strBuffer.CopyTo(obuffer, 0);

            socket.Send(obuffer, strBuffer.Length, SocketFlags.None);
        }
    }
}
