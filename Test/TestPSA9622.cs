using System;
using Microsoft.SPOT;
using MFCommon.Hardware.I2C;

namespace MFCommon.Test
{
    public class TestPSA9622
    {
        public static void Main()
        {
            PCA9622 device = new PCA9622();

            device.Address = 123;
            device.SetDutyCycle(PCA9622.Output.LED_0, 100);

        }
    }
}
