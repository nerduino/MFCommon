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
