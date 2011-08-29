using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace MFCommon.Hardware
{
    public class AD5206 : SharedSPI
    {
        public AD5206(Cpu.Pin csPin) : base(csPin) {
        }
        
        public void SetWiper(byte channel, byte position) 
        {
            if (channel < 0 || channel > 5) throw new ArgumentOutOfRangeException("Channel 0-6");
            if (position < 0 || position > 255) throw new ArgumentOutOfRangeException("Position 0-255");
            base.Write(channel, position);
       }

        public void SetWiper(int channel, int resistance)
        {
            SetWiper((byte)channel, (byte)resistance);
        }
    }

    public class AD5206_Channel
    {

        public byte Channel { get; set; }
        private AD5206 Output { get; set; }
        public byte Wiper
        {
            set
            {
                Output.SetWiper(Channel, value);
            }
        }

        public AD5206_Channel(AD5206 output, byte channel)
        {
            this.Output = output;
            this.Channel = channel;
        }
    }
}
