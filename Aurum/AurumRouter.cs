using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum
{
    class AurumRouter
    {
        private Socket NodeSocket;
        private List<Socket> Channels = new List<Socket>();
        private string RouteHost;
        private ushort RoutePort;

        public AurumRouter(Socket nodeSocket, string localHost, ushort localPort)
        {
            NodeSocket = nodeSocket;
            RouteHost = localHost;
            RoutePort = localPort;
        }

        private Socket NewChannel()
        {
            var chan = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            chan.Connect(RouteHost, RoutePort);
            return chan;
        }

        public void Route()
        {
            int outSpd = 0;
            int inSpd = 0;
            var buffIn = new byte[65537];
            var buffOut = new byte[65537];
            bool state = true;

            Task.Run(() =>
            {
                while (state)
                {
                    var recd = NodeSocket.Receive(buffOut);
                    inSpd += recd;
                    if (buffOut[0] >= Channels.Count)
                    {
                        Channels.Insert(buffOut[0], NewChannel());
                        ComHelper.Warn($"Channel {buffOut[0]} created");
                    }
                    Channels[buffOut[0]].Send(buffOut, 1, recd - 1, SocketFlags.None);
                }
            });

            Task.Run(() =>
            {
                while (state)
                {
                    if(Channels.Count > 0)
                    {
                        for (byte z = 0; z < Channels.Count; z++)
                        {
                            var recd = Channels[z].Receive(buffIn, 1, 65536, SocketFlags.None);
                            buffIn[0] = z;
                            outSpd += recd;
                            NodeSocket.Send(buffIn, 0, recd + 1, SocketFlags.None);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                    
                }
            });

            while (state)
            {
                Console.SetCursorPosition(0, 8);
                ComHelper.Succ($"{NodeSocket.RemoteEndPoint} -> {NodeSocket.LocalEndPoint} (TX:{ComHelper.GetReducedSize(inSpd)}/s, RX:{ComHelper.GetReducedSize(outSpd)}/s)                ");
                outSpd = 0; inSpd = 0;
                Thread.Sleep(1000);
            }
        }
    }
}
