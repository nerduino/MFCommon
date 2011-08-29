using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace MFCommon.Hardware
{
    public abstract class SharedSPI
    {
        public static SPI SPI { get; set; }
        public SPI.Configuration Configuration { get; set; }

        public SharedSPI(Cpu.Pin csPin)
        {
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

        protected void Write(byte channel, byte value)
        {

            byte[] WriteBuffer = new byte[2];
            WriteBuffer[0] = channel;
            WriteBuffer[1] = value;

            SPI.Config = Configuration;
            SPI.Write(WriteBuffer);
        }
    }
}
