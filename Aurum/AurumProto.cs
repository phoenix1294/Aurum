using System;
using System.Net.Sockets;

namespace Aurum
{
    enum ReservedChannels
    {
        Handshake = 0x00,
        ChannelDefine = 0x01
    }

    class AurumProto
    {
        public static AurumPackage ReceivePackage(Socket socket)
        {
            var size = new byte[4];
            var channel = new byte[4];
            var data = new byte[65536];
            socket.Receive(size, 0x04, SocketFlags.None);
            socket.Receive(channel, 0x04, SocketFlags.None);

            var int_size = BitConverter.ToInt32(size);
            socket.Receive(data, int_size, SocketFlags.None);
            return new AurumPackage(BitConverter.ToInt32(channel), data[..int_size]);
        }

        public static void SendPackage(AurumPackage package, Socket socket)
        {
            var size = BitConverter.GetBytes(package.Length);
            var channel = BitConverter.GetBytes(package.Channel);
            socket.Send(size);
            socket.Send(channel);
            socket.Send(package.GetData(), package.Length, SocketFlags.None);
        }
    }
}
