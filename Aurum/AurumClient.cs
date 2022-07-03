using System.Net.Sockets;

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
            ComHelper.Succ("All successfully connected");
            var router = new AurumRouter(nodeSocket, MappingHost, MappingPort);
            router.Route();
        }
    }
}
