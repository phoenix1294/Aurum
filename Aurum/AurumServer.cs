using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Aurum
{

    class AurumServer
    {
        private readonly ushort NodePort;

        private List<Socket> Clients = new List<Socket>();
        private List<Socket> Connections = new List<Socket>();

        private byte CurrentClients;
        private byte CurrentConnections;

        private Random Rand = new Random();

        public AurumServer(ushort nodePort)
        {
            NodePort = nodePort;
        }

        private bool CheckWhiteIP()
        {
            // some mechanisms to check 
            return true;
        }

        public void Run()
        {
            Console.Clear();
            TcpListener nodeServer = new TcpListener(NodePort); // client accepter
            bool success = false;
            try
            {
                nodeServer.Start();
                ComHelper.Succ($"Aurum Server started on port {NodePort}");
                success = true;
            }
            catch (SocketException)
            {
                ComHelper.Err($"Port {NodePort} isn't available");
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
                        ComHelper.Warn($"Client at {Clients[thisClient].RemoteEndPoint} was fall");
                    }
                });
            }
        }

        private void CreateSession(Socket clientSocket)
        {
            var sessionChannel = Rand.Next(8192, 32768);
            bool success = false;

            var handshake = AurumProto.ReceivePackage(clientSocket);
            if (handshake.Channel == (int)ReservedChannels.Handshake)
            {
                var identifier = Encoding.UTF8.GetString(handshake.GetData());
                ComHelper.Log($"Client \'{identifier}\' connected");
                AurumProto.SendPackage(new AurumPackage(ReservedChannels.ChannelDefine, sessionChannel), clientSocket);
                TcpListener listener = new TcpListener(sessionChannel);
                try
                {
                    listener.Start();
                    ComHelper.Succ($"Channel {sessionChannel} allocated by {identifier}");
                    success = true;
                }
                catch (SocketException)
                {
                    ComHelper.Err($"Port {sessionChannel} isn't available");
                }

                while (success)
                {
                    var router = new AurumRoute(clientSocket, listener.AcceptSocket(), 6);
                    router.Route();
                }
            }
            else
            {
                ComHelper.Err("Handshake was broken");
            }
        }
    }
}
