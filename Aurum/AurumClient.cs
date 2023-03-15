using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum
{
    class AurumClient
    {
        private ushort MappingPort;
        private string MappingHost;
        private ushort NodePort;
        private string NodeHost;
        private int Channel;

        public AurumClient(string host, ushort port)
        {
            MappingPort = port;
            NodeHost = host;
            MappingHost = "127.0.0.1"; // HARDCODE
            NodePort = 2418; // HARDCODE!
        }

        public void Run()
        {
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(NodeHost, NodePort); // Connection to Aurum Server

            AurumProto.SendPackage(new AurumPackage(ReservedChannels.Handshake, Encoding.UTF8.GetBytes("sodium")), serverSocket);
            var handshake = AurumProto.ReceivePackage(serverSocket);

            if (handshake.Channel == (int)ReservedChannels.ChannelDefine)
            {
                Channel = BitConverter.ToInt32(handshake.GetData());
                ComHelper.Succ($"Channel {Channel} allocated!");
                
                var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(MappingHost, MappingPort); // Connection to ClientSide

                Task.Run(() =>
                {
                    while (true)
                    {
                        if (!serverSocket.Connected)
                        {
                            serverSocket.Connect(NodeHost, NodePort); // Connection to Aurum Node
                        }
                        else
                        {
                            Thread.Sleep(50);
                        }
                    }
                });

                ComHelper.Succ("All successfully connected");
                var router = new AurumRoute(serverSocket, clientSocket, 6);
                router.Route();
            }
            else
            {
                ComHelper.Err("Handshake is broken");
            }
            
        }
    }
}
