using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum
{
    class AurumNodeRouter
    {
        private Socket NodeSocket;
        private List<Socket> Channels = new List<Socket>();

        public AurumNodeRouter(Socket nodeSocket)
        {
            NodeSocket = nodeSocket;
        }

        public void AddChannel(Socket chan)
        {
            Channels.Add(chan);
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
                    if(Channels.Count > 0)
                    {
                        var recd = NodeSocket.Receive(buffOut, 0, 65536, SocketFlags.None);
                        inSpd += recd;
                        Channels[buffOut[0]].Send(buffOut, 1, recd - 1, SocketFlags.None);
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            });

            Task.Run(() =>
            {
                while (state)
                {
                    if (Channels.Count > 0)
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
