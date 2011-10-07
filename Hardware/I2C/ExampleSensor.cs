using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using MFCommon.Hardware;
using MFCommon.Hardware.I2C;

namespace i2cExample
{
    /// <summary>
    /// This is an I2C sensor.
    /// </summary>
    public class ExampleSensor
    {
        private I2CDevice.Configuration _slaveConfig;
        private const int TransactionTimeout = 1000; // ms
        private const byte ClockRateKHz = 59;
        public byte Address { get; private set; }

        /// <summary>
        /// Example sensor constructor
        /// </summary>
        /// <param name="address">I2C device address of the example sensor</param>
        public ExampleSensor(byte address)
        {
            Address = address;
            _slaveConfig = new I2CDevice.Configuration(address, ClockRateKHz);
        }


        public byte[] ReadSomething()
        {
            // write register address
            I2CBus.GetInstance().Write(_slaveConfig, new byte[] {0xF2}, TransactionTimeout);

            // get MSB and LSB result
            byte[] data = new byte[2];
            I2CBus.GetInstance().Read(_slaveConfig, data, TransactionTimeout);

            return data;
        }

        public byte[] ReadSomethingFromSpecificRegister()
        {
            // get MSB and LSB result
            byte[] data = new byte[2];
            I2CBus.GetInstance().ReadRegister(_slaveConfig, 0xF1, data, TransactionTimeout);

            return data;
        }

        public void WriteSomethingToSpecificRegister()
        {
            I2CBus.GetInstance().WriteRegister(_slaveConfig, 0x3C, new byte[2] { 0xF4, 0x2E }, TransactionTimeout);
        }


    }
}
