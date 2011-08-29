
/*
 * Servo NETMF Driver
 *&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Coded by Chris Seto August 2010
 *&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <chris@chrisseto.com>
 *
 * Use this code for whatveer you want. Modify it, redistribute it, I don't care.
 * I do ask that you please keep this header intact, however.
 * If you modfy the driver, please include your contribution below:
 *
 * Chris Seto: Inital release (1.0)
 * Chris Seto: Netduino port (1.0 -> Netduino branch)
 * Chris Seto: bool pin state fix (1.1 -> Netduino branch)
 *
 *
 * */

using System;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;

namespace MFCommon.Hardware
{
    public class Servo : IDisposable
    {
        /// <summary>
        /// PWM handle
        /// </summary>
        private PWM servo;

        /// <summary>
        /// Timings range
        /// </summary>
        private int[] range = new int[2];

        /// <summary>
        /// Set servo inversion
        /// </summary>
        public bool inverted = false;

        /// <summary>
        /// Create the PWM pin, set it low and configure timings
        /// </summary>
        /// <param name="pin"></param>
        public Servo(Cpu.Pin pin)
        {
            // Init the PWM pin
            servo = new PWM((Cpu.Pin)pin);

            servo.SetDutyCycle(0);

            // Typical settings
            range[0] = 1000;
            range[1] = 2000;
        }

        public void Dispose()
        {
            disengage();
            servo.Dispose();
        }

        /// <summary>
        /// Allow the user to set cutom timings
        /// </summary>
        /// <param name="fullLeft"></param>
        /// <param name="fullRight"></param>
        public void setRange(int fullLeft, int fullRight)
        {
            range[1] = fullLeft;
            range[0] = fullRight;
        }

        /// <summary>
        /// Disengage the servo.
        /// The servo motor will stop trying to maintain an angle
        /// </summary>
        public void disengage()
        {
            // See what the Netduino team say about this...
            servo.SetDutyCycle(0);
        }

        /// <summary>
        /// Set the servo degree
        /// </summary>
        public double Degree
        {
            set
            {
                /// Range checks
                if (value > 180)
                    value = 180;

                if (value < 0)
                    value = 0;

                // Are we inverted?
                if (inverted)
                    value = 180 - value;

                // Set the pulse
                servo.SetPulse(20000, (uint)map((long)value, 0, 180, range[0], range[1]));
            }
        }

        /// <summary>
        /// Used internally to map a value of one scale to another
        /// </summary>
        /// <param name="x"></param>
        /// <param name="in_min"></param>
        /// <param name="in_max"></param>
        /// <param name="out_min"></param>
        /// <param name="out_max"></param>
        /// <returns></returns>
        private long map(long x, long in_min, long in_max, long out_min, long out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}
