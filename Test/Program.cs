using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using MFCommon.Hardware;

namespace Test
{
    public class Program
    {
        public static void Main()
        {
            int channel = 0, position = 0;
            int delta = 1;
            AD5206 ad5206 = new AD5206(Pins.GPIO_PIN_D10);

            ad5206.SetWiper(channel, 0);
            Thread.Sleep(5000);
            ad5206.SetWiper(channel, 255);
            Thread.Sleep(5000);

            while (true)
            {
                Debug.Print("Position: " + position);
                ad5206.SetWiper(channel, position);
                position += delta;

                if (position >= 255 || position == 0)
                {
                    delta = delta * -1;
                }
                
                Thread.Sleep(100);
            }
        }
    }
}
