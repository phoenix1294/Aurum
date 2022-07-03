using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum
{
    class NetworkHelper
    {
        public static void Route(Socket nodeSocket, Socket socketB, int consoleLine)
        {
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
                        var recd = nodeSocket.Receive(buffOut);
                        inSpd += recd;
                        socketB.Send(buffOut, 0, recd, SocketFlags.None);
                    }
                    catch (Exception e)
                    {
                        ComHelper.Err(e.Message);
                        state = false;
                        break;
                    }

                }
            });

            Task.Run(() =>
            {
                while (state)
                {
                    try
                    {
                        var recd = socketB.Receive(buffIn);
                        outSpd += recd;
                        nodeSocket.Send(buffIn, 0, recd, SocketFlags.None);
                    }
                    catch (Exception e)
                    {
                        ComHelper.Err(e.Message);
                        state = false;
                        break;
                    }
                }
            });

            while (state)
            {
                Console.SetCursorPosition(0, consoleLine);
                Console.Write($"{nodeSocket.RemoteEndPoint} -> {nodeSocket.RemoteEndPoint} (TX:{ComHelper.GetReducedSize(inSpd)}/s, RX:{ComHelper.GetReducedSize(outSpd)}/s)                ");
                outSpd = 0; inSpd = 0;
                Thread.Sleep(1000);
            }
        }
    }
}
