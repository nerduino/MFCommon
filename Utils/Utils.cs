using System;
using Microsoft.SPOT;

namespace MFCommon.Utils
{
    public class Log
    {
        public static void Debug(string message)
        {
            Microsoft.SPOT.Debug.Print(message);
        }
    }
}
