using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoPixel
{
    public static class NeoPixelNative
    {
        /// <summary>
        /// This is the interop call to the low level native C++ code that resides in the modified firmware
        /// The firmware must contain the NeoPixel low level native C++ code
        /// This method is "internal" so that NeoPixelChain may access it
        /// </summary>
        /// <param name="dataPtr">array of bytes already organized in the GRB format, ready to be sent to all the NeoPixels</param>
        /// <param name="count">the number of NeoPixels</param>
        /// <param name="pin">The Cpu.Pin representation of which MCU pin the first NeoPixel's data input pin is connected to</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void Write(byte[] dataPtr, int count, UInt32 pin);
    }
}
