using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Aurum
{
    class AurumNode
    {
        private readonly byte MaxClients;
        private readonly ushort NodePort;


        private List<Socket> Clients = new List<Socket>();
        private List<Socket> Connections = new List<Socket>();

        private byte CurrentClients;
        private byte CurrentConnections;

        private Random Rand = new Random();

        public AurumNode(byte maxusers, ushort nodePort)
        {
            MaxClients = maxusers;
            NodePort = nodePort;
        }

        public void Run()
        {
            Console.Clear();
            TcpListener nodeServer = new TcpListener(NodePort); // client accepter
            bool success = false;
            try
            {
                nodeServer.Start();
                ComHelper.Succ($"Aurum node started on port {NodePort}");
                success = true;
            }
            catch (SocketException)
            {
                Console.WriteLine($"Port {NodePort} isn't available");
            }
            if(success)
            {
                ServerLoop(nodeServer);
            }
        }

        private void ServerLoop(TcpListener nodeServer)
        {
            while (true)
            {
                Clients.Add(nodeServer.AcceptSocket());
                Task.Run(() =>
                {
                    var thisClient = CurrentClients;
                    try
                    {
                        CreateSession(Clients[thisClient]);
                        CurrentClients++;
                    }
                    catch
                    {
                        Console.WriteLine($"Client at {Clients[thisClient].RemoteEndPoint} was fall");
                    }
                });
            }
        }

        private void CreateSession(Socket nodeSocket)
        {
            var sessionPort = 24816; //Rand.Next(8192, 32768);
            bool success = false;
            ComHelper.Log($"Client connected");
            

            TcpListener listener = new TcpListener(sessionPort);
            try
            {
                listener.Start();
                ComHelper.Succ($"Port {sessionPort} is open");
                success = true;
            }
            catch (SocketException)
            {
                ComHelper.Err($"Port {sessionPort} isn't available");
            }

            while(success)
            {
                var router = new AurumRoute(nodeSocket, listener.AcceptSocket(), 6);
                router.Route();
            }
        }
    }
}
