using System;
using Microsoft.SPOT;

namespace MFCommon.Hardware.I2C
{
    /// <summary>
    /// An NXP 16 channel 8 bit PWM I2C LED Driver 100ma 40V outputs
    /// </summary>
    public class PCA9622 : I2CBaseDevice
    {
        
        public const byte REGISTER_MASK = 0x1F;

        public enum DefaultBusAddress : byte
        {
            ALLCALLADR = 0xE0,
            SUBADR1 = 0xE2,
            SUBADR2 = 0xE4,
            SUBADR3 = 0xE8
        }
        public enum Register : byte
        {
            MODE1 = 0x00,
            MODE2 = 0x01,
            GRPPWM = 0x12,
            GRPFREQ = 0x13,
            LEDOUT0 = 0x14,
            LEDOUT1 = 0x15,
            LEDOUT2 = 0x16,
            LEDOUT3 = 0x17,
            SUBADR1 = 0x18,
            SUBADR2 = 0x19,
            SUBADR3 = 0x1A,
            ALLCALLADR = 0x20
        }

        public enum Output : byte
        {
            LED_0 = 0x02,
            LED_1 = 0x03,
            LED_2 = 0x04,
            LED_3 = 0x05,
            LED_4 = 0x06,
            LED_5 = 0x07,
            LED_6 = 0x08,
            LED_7 = 0x09,
            LED_8 = 0x0A,
            LED_9 = 0x0B,
            LED_A = 0x0C,
            LED_B = 0x0D,
            LED_C = 0x0E,
            LED_D = 0x0F,
            LED_E = 0x10,
            LED_F = 0x11
        }

        public enum AutoIncrementOptions : byte
        {
            NO_INCREMENT = 0,
            AUTO_ALL_ = 4,
            AUTO_BRIGHTNESS = 5,
            AUTO_INCREMENT_GLOBAL = 6,
            AUTO_BRIGHTNESS_AND_GLOBAL = AUTO_BRIGHTNESS | AUTO_INCREMENT_GLOBAL
        }

        public enum DriverState : byte
        {
            OFF = 0x00,
            ON = 0x01,
            PWM = 0x02,
            PWM_AND_GRP = 0x04
        }

        /// <summary>
        /// Write a byte to a register, clearing any auto increment options.
        /// </summary>
        /// <param name="register"></param>
        /// <param name="value"></param>
        public void SetRegister(Register register, byte value)
        {
            base.Write(register.Masked(), value);
        }

        public void SetDutyCycle(Output output, int percent)
        {
            byte value = (byte) ( (percent / 100) * 256 );
            base.Write((byte)output, value);
        }
    }


    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        public static byte Masked(this MFCommon.Hardware.I2C.PCA9622.Register register)
        {
            return (byte)((byte)register & PCA9622.REGISTER_MASK);
        }

        public static byte Masked(this MFCommon.Hardware.I2C.PCA9622.Register register, MFCommon.Hardware.I2C.PCA9622.AutoIncrementOptions autoOptions)
        {
            return (byte) ( ((byte)register & PCA9622.REGISTER_MASK) | ((byte)autoOptions << 5) );
        }

    }
}