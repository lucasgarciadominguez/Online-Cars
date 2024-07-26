using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESNEServer23
{
    internal class Listener
    {
        public TcpListener listener;

        public Listener(Pool pool, int port)
        {
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");
            listener = new TcpListener(localAddress, port);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                client.Client.Blocking = false;

                if (!pool.AddConnection(client))
                    client.Close();
            }
        }
    }
}
