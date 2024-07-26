using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESNEServer23
{
    internal class Pool
    {
        private int maxConnections;
        private List<Connection> connections = new List<Connection>();
        int numberOfConnections;
        int timeForWaitingForPlayers = 10;
        bool isWaitingForPlayers=true;
        bool firstConnection=false;
        Thread counterThread;
        string startRace = "startRace|";
        public Pool(int maxConnections)
        {
            this.maxConnections = maxConnections;
        }

        public bool AddConnection(TcpClient client)
        {
            if (connections.Count >= maxConnections)
                return false;
            if (isWaitingForPlayers)
            {
                if (!firstConnection)
                {
                    Connection connection = new Connection(client);
                    connection.SendToAllDelegate = SendToAll;
                    connection.SendToAllButThisDelegate = SendToAllButThis;
                    firstConnection = true;
                    lock (connections)
                    {
                        connection.numberOfConnection = numberOfConnections;
                        connections.Add(connection);
                        Console.WriteLine("New connection: " + connection + " number of player: " + numberOfConnections);

                        numberOfConnections++;
                        return true;
                    }
                }
                else
                {

                    Connection connection = new Connection(client);
                    connection.SendToAllDelegate = SendToAll;
                    connection.SendToAllButThisDelegate = SendToAllButThis;

                    counterThread = new Thread(Counter);
                    counterThread.Start();
                    lock (connections)
                    {
                        connection.numberOfConnection = numberOfConnections;
                        connections.Add(connection);
                        Console.WriteLine("New connection: " + connection + " number of player: " + numberOfConnections);

                        numberOfConnections++;
                        return true;
                    }
                }
            }
            else
                return false;   //not connected, race has started


        }
        public void Counter()
        {
            int counter = 0;

            // Bucle infinito para que el contador se incremente cada segundo
            while (isWaitingForPlayers)
            {
                //Console.Clear(); // Borra la consola para actualizar el contador

                // Imprime el contador en la consola
                Console.WriteLine("Counter: " + counter);

                // Incrementa el contador
                counter++;

                // Espera 1 segundo antes de la próxima iteración
                System.Threading.Thread.Sleep(1000);
                if (counter >= timeForWaitingForPlayers)
                {
                    isWaitingForPlayers = false;
                    SendToAll(startRace);

                    foreach (var connection in connections)
                    {
                        connection.startCountdown = true;

                    }
                }
            }
        }
        public void RemoveConnection(Connection connection)
        {
            lock (connections)
            {
                numberOfConnections--;
                connection.socket.Close();
                Console.WriteLine("Removing connection: " + connection);
                connections.Remove(connection);
            }
        }

        public void Process()
        {
            List<Connection> connectionsToClose = new List<Connection>();

            lock (connections)
            {
                foreach (Connection connection in connections)
                {
                    if (!connection.Process())
                        connectionsToClose.Add(connection);
                }
            }

            foreach (Connection connection in connectionsToClose)
                RemoveConnection(connection);
        }

        private void SendToAll(string str)
        {
            lock(connections)
            {
                foreach (Connection connection in connections)
                {
                    connection.Send(str);
                }
            }
        }

        private void SendToAllButThis(string str, Connection sender)
        {
            lock (connections)
            {
                foreach (Connection connection in connections)
                {
                    if (connection != sender)
                        connection.Send(str);
                }
            }
        }

    }
}
