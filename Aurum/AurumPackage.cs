using System;
using System.Text;

namespace Aurum
{
    class AurumPackage
    {
        public int Length { get; private set; }
        public int Channel { get; private set; }
        private byte[] Data;

        public AurumPackage(int channel, byte[] data)
        {
            Data = data;
            Channel = channel;
            Length = data.Length;
        }

        public AurumPackage(ReservedChannels channel, byte[] data)
        {
            Data = data;
            Channel = (int)channel;
            Length = data.Length;
        }

        public AurumPackage(ReservedChannels channel, string data)
        {
            Data = Encoding.UTF8.GetBytes(data);
            Channel = (int)channel;
            Length = data.Length;
        }

        public AurumPackage(ReservedChannels channel, int data)
        {
            Data = BitConverter.GetBytes(data);
            Channel = (int)channel;
            Length = 0x04;
        }

        public byte[] GetData()
        {
            return Data;
        }

        public void SetData(byte[] data)
        {
            Data = data;
            Length = data.Length;
        }
    }
}
