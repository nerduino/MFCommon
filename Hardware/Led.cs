using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace MFCommon.Hardware
{
    public class Led
    {
        public bool State { get; set; }
        public int FlashDelay { get; set; }

        private OutputPort port;
        private Cpu.Pin pin;
        private bool p;
        private int p_2;

        public Led(Cpu.Pin portId, bool initialState)
        {
            port = new OutputPort(portId, initialState);
            State = initialState;
        }

        public Led(Cpu.Pin pin, bool initialState, int flashDelay) : this(pin, initialState)
        {
            FlashDelay = flashDelay;
        }
        
        public virtual void Write(bool state) {
            port.Write(state);
            State = state;
        }


        public void Flash()
        {
            Write(!State);
            Thread.Sleep(FlashDelay);
            Write(State);
        }

        public void Flash(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Flash();
                Thread.Sleep(FlashDelay);
            }
        }
    }
}
