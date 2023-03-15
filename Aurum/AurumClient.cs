using System.Net.Sockets;
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

        public AurumClient(string host, ushort port)
        {
            MappingPort = port;
            NodeHost = host;
            MappingHost = "127.0.0.1";
            NodePort = 2418;
        }

        public void Run()
        {
            var nodeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            nodeSocket.Connect(NodeHost, NodePort); // Connection to Aurum Node

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(MappingHost, MappingPort); // Connection to ClientSide

            Task.Run(() =>
            {
                while(true)
                {
                    if(!nodeSocket.Connected)
                    {
                        nodeSocket.Connect(NodeHost, NodePort); // Connection to Aurum Node
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
            });

            ComHelper.Succ("All successfully connected");
            var router = new AurumRoute(nodeSocket, clientSocket, 6);
            router.Route();
        }
    }
}
