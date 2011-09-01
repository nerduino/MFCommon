using System;
using Microsoft.SPOT;

namespace MFCommon.Hardware
{
    public class Stopwatch
    {
        private long startTicks = 0;
        private long stopTicks = 0;
        private bool isRunning = false;

        private const long ticksPerMillisecond = System.TimeSpan.TicksPerMillisecond;

        public Stopwatch()
        {
        }

        public void Reset()
        {
            startTicks = 0;
            stopTicks = 0;
            isRunning = false;
        }

        public void Start()
        {
            if (startTicks != 0 && stopTicks != 0)
            {
                startTicks = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - (stopTicks - startTicks); // resume existing timer
            }
            else
            {
                startTicks = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks; // start new timer
            }
            isRunning = true;
        }

        public void Stop()
        {
            stopTicks = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            isRunning = false;
        }

        public long ElapsedMilliseconds
        {
            get
            {
                if (startTicks != 0 && isRunning)
                {
                    return (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - startTicks) / ticksPerMillisecond;
                }
                else if (startTicks != 0 && !isRunning)
                {
                    return (stopTicks - startTicks) / ticksPerMillisecond;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
