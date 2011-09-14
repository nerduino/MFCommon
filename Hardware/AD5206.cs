using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace MFCommon.Hardware
{
    public class AD5206 : SharedSPI
    {

        public AD5206(Cpu.Pin csPin)  {
            Configuration = new SPI.Configuration(
               csPin,             // /CS pin
               false,             // /CS active LOW
               0,                 // /CS setup time
               0,                 // /CS hold time
               false,             // clock low on idle
               true,              // rising edge data
               1000,              // SPI clock rate in KHz
               SPI_Devices.SPI1   // MOSI MISO and SCLK pinset
           );

            if (SPI == null)
            {
                SPI = new SPI(Configuration);
            }
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
        
        private byte wiper;

        public byte Wiper
        {
            set
            {
                this.wiper = value;
                Output.SetWiper(Channel, wiper);
            }
            get
            {
                return wiper;
            }
        }

        public AD5206_Channel(AD5206 output, byte channel)
        {
            this.Output = output;
            this.Channel = channel;
        }
    }
}
