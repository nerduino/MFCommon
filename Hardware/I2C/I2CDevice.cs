using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MFCommon.Hardware.I2C
{
    /// <summary>
    /// Base class for I2C devices
    /// </summary>
    public abstract class I2CBaseDevice
    {
        /// <summary>
        /// Device address
        /// </summary>
        public byte Address { get; set; }

        /// <summary>
        /// Bus transaction timeout
        /// </summary>
        public int TransactionTimeout { get; set; }

        /// <summary>
        /// Clock rate in KHz
        /// </summary>
        public int ClockRate { get; set; }

        protected I2CDevice.Configuration config;

        public I2CBaseDevice(byte address = 0, int transactionTimeout = 1000, int clockRate = 100)
        {
            this.Address = address;
            this.TransactionTimeout = transactionTimeout;
            this.ClockRate = clockRate;

            config = new I2CDevice.Configuration(Address, ClockRate);
        }

        protected void Write(byte register, byte value)
        {
            I2CBus.GetInstance().WriteRegister(config, register, value, TransactionTimeout);
        }

        protected void Write(byte register, byte[] value)
        {
            I2CBus.GetInstance().WriteRegister(config, register, value, TransactionTimeout);
        }

        protected byte[] Read(byte register)
        {
            byte[] readBuffer = new byte[2];
            I2CBus.GetInstance().ReadRegister(config, register, readBuffer, TransactionTimeout);

            return readBuffer;
        }

        protected byte ReadByte(byte register)
        {
            byte[] readBuffer = new byte[1];
            I2CBus.GetInstance().ReadRegister(config, register, readBuffer, TransactionTimeout);

            return readBuffer[0];
        }
    }
}
