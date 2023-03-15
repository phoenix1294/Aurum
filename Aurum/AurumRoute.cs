using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum
{
    class AurumRoute
    {
        private Socket SocketA;
        private Socket SocketB;
        private readonly int Line;

        private bool Busy = false;

        public AurumRoute(Socket socketA, Socket socketB, int line)
        {
            SocketA = socketA;
            SocketB = socketB;
            Line = line;
        }

        public bool IsBusy()
        {
            return Busy;
        }

        public void Route()
        {
            Busy = true;
            int outSpd = 0;
            int inSpd = 0;
            var buffIn = new byte[65536];
            var buffOut = new byte[65536];
            bool state = true;

            Task.Run(() =>
            {
                while (state)
                {
                    try
                    {
                        var recd = SocketA.Receive(buffIn);
                        inSpd += recd;
                        SocketB.Send(buffIn, recd, SocketFlags.None);
                    }
                    catch
                    {
                        ComHelper.Err("RX was fall");
                        state = false;
                    }
                }
            });

            Task.Run(() =>
            {
                while (state)
                {
                    try
                    {
                        var recd = SocketB.Receive(buffOut);
                        outSpd += recd;
                        SocketA.Send(buffOut, recd, SocketFlags.None);
                    }
                    catch
                    {
                        ComHelper.Err("TX was fall");
                        state = false;
                    }
                }
            });

            while (state)
            {
                Console.SetCursorPosition(0, Line);
                ComHelper.Succ($"{SocketA.RemoteEndPoint} -> {SocketB.LocalEndPoint} (TX:{ComHelper.GetReducedSize(inSpd)}/s, RX:{ComHelper.GetReducedSize(outSpd)})/s                ");
                outSpd = 0; inSpd = 0;
                Thread.Sleep(1000);
            }
            Busy = false;
        }
    }
}
