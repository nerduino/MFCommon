using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MFCommon.Hardware.I2C
{
    

    /// <summary>
    /// An NXP 16 channel 12 bit PWM I2C LED Driver
    /// </summary>
    public class PCA6985 : I2CBaseDevice
    {
        public enum Addresses
        {
            ALLCALLADR = 0xE0,
            SWRESET = 0x06
        }
       /// <summary>
       /// Device Registers - refer to datasheet
       /// </summary>
        public enum Register
        {
            MODE1 = 0x00,
            MODE0 = 0x01,
            SUBADR1 = 0x02,
            SUBADR2 = 0x03,
            SUBADR3 = 0x04,
            ALLCALLADR = 0x05,
            PRESCALE = 0xFE,
            TESTMODE = 0xFF 

        }
        /// <summary>
        /// Base address for brightness control registers
        /// Base = ON Low Byte
        /// Base + 1 = On High Byte
        /// Base + 2 = Off Low Byte
        /// Base + 3 = Off High Byte
        /// 
        /// </summary>
        public enum Output
        {
            LED_0 = 0x06,
            LED_1 = 0x0A,
            LED_2 = 0x0E,
            LED_3 = 0x12,
            LED_4 = 0x16,
            LED_5 = 0x1A,
            LED_6 = 0x1E,
            LED_7 = 0x22,
            LED_8 = 0x26,
            LED_9 = 0x2A,
            LED_A = 0x2E,
            LED_B = 0x32,
            LED_C = 0x36,
            LED_D = 0x3A,
            LED_E = 0x3E,
            LED_F = 0x42,
            LED_ALL = 0xFA
        }

        public enum OutputStructure
        {
            OPEN_DRAIN = 0, 
            TOTEM_POLE = 1
        }

        public OutputStructure CurrentOutputStructure {
            get {
                byte data = ReadByte((byte)Register.MODE1);
                return (data & 0x04) == 1 ? OutputStructure.TOTEM_POLE : OutputStructure.OPEN_DRAIN;
            }
            set{
                byte data = ReadByte((byte)Register.MODE1);
                Write((byte)Register.MODE1, (byte)(data | ((byte)value << 4)) );
            }

        } 

        /// <summary>
        /// Set the output low and high values.
        /// (see datasheet for definition)
        /// </summary>
        /// <param name="output">The Output register</param>
        /// <param name="on">The on start count</param>
        /// <param name="off">The off start count</param>
        public void SetOutput(Output output, int on, int off)
        {
            byte register = (byte)output;

            byte[] data = new byte[4];
            data[0] = (byte)(on & 0xff);
            data[1] = (byte)((on >> 8) & 0xff);
            data[2] = (byte)(off & 0xff);
            data[3] = (byte)((off >> 8) & 0xff);

            foreach (var b in data)
            {
                Write(register++, b);    
            }
        }

        /// <summary>
        /// Set an output dutycycle to the given percent
        /// </summary>
        /// <param name="output"></param>
        /// <param name="percent"></param>
        public void SetOutput(Output output, int percent)
        {
            int highStart = (4096 * percent / 100) + 1;
            SetOutput(output, 0, highStart);
        }
        

        /// <summary>
        /// Set an output either on or off
        /// </summary>
        /// <param name="output"></param>
        /// <param name="state"></param>
        public void SetOutput(Output output, bool state)
        {
            byte register = (byte)output;

            //ON[12] = 1, OFF[12] = !state;
            
            byte on = 0x08;
            byte off = (byte)(state ? 0x00: 0x08);
            
            Write(register++, 0);
            Write(register++, on);
            Write(register++, 0);
            Write(register, off);
        }

    }
}
