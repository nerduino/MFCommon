using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace MFCommon.Hardware
{
    public class Led
    {
        public bool State { get; set; }

        private OutputPort port;

        public Led(Cpu.Pin portId, bool initialState)
        {
            port = new OutputPort(portId, initialState);
            State = initialState;
        }
        
        public virtual void Write(bool state) {
            port.Write(state);
            State = state;
        }


        public void Flash()
        {
            Write(!State);
            Thread.Sleep(100);
            Write(State);
        }

        public void Flash(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Flash();
                Thread.Sleep(100);
            }
        }
    }
}
