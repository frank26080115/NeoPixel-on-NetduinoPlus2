using System;
using System.Collections;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace NeoPixel
{
    /// <summary>
    /// This class contains utilities to handle basic writing to a set of NeoPixels.
    /// It also contains the interop call to the low level native C++ code.
    /// 
    /// The user is responsible for organizing the set of NeoPixels while using this class,
    /// because most of the methods in this class takes arrays as parameters.
    /// 
    /// NeoPixelChain is another class that helps the user organize the set of NeoPixels,
    /// see the comments for NeoPixelChain for more details
    /// 
    /// Note: generic object arrays are not accepted as parameters, only arrays of NeoPixels and Colors are allowed
    /// 
    /// Note: this class does not resemble System.IO style streams
    /// </summary>
    public class NeoPixelWriter
    {
        /// <summary>
        /// remembers which pin
        /// </summary>
        private Cpu.Pin pin;

        /// <summary>
        /// references the pin, may have already been initialized
        /// </summary>
        private OutputPort op;

        /// <summary>
        /// Constructor that initializes a single pin that is connected to the first NeoPixel's data input
        /// </summary>
        /// <param name="pin">MCU pin that is connected to the first NeoPixel's data input</param>
        public NeoPixelWriter(Cpu.Pin pin)
        {
            this.pin = pin;
            op = new OutputPort(pin, false);
        }

        /// <summary>
        /// Similar to the other constructor assumes that the pin is pre-initialized
        /// </summary>
        /// <param name="pin">MCU pin that is connected to the first NeoPixel's data input, must be pre-initialized or initialized before "Write"</param>
        public NeoPixelWriter(OutputPort pin)
        {
            this.pin = op.Id;
            op = pin;
        }

        /// <summary>
        /// Updates a set of NeoPixels
        /// </summary>
        /// <param name="pixels">a list of NeoPixels to update, index 0 is the first NeoPixel connected to the MCU</param>
        /// <param name="count">the number of NeoPixels</param>
        public void Write(NeoPixel[] pixels, int count)
        {
            byte[] barr = new byte[count * 3];
            for (int i = 0; i < count; i++)
            {
                // the order here is critical, it matches what the NeoPixels expect
                barr[i * 3 + 0] = pixels[i].Green;
                barr[i * 3 + 1] = pixels[i].Red;
                barr[i * 3 + 2] = pixels[i].Blue;
            }
            NeoPixelNative.Write(barr, count, (UInt32)pin);
        }

        /// <summary>
        /// The "Color" enum is a used for generic graphic stuff inside Microsoft.SPOT
        /// This method allows you to set NeoPixels using Microsoft.SPOT's "Color"
        /// Note: the "Color" enum is a 32 bit integer but GRB is 24 bit, however there are no data types available for just 24 bits
        /// </summary>
        /// <param name="pixels">a list of colors to be written, index 0 is the first NeoPixel connected to the MCU</param>
        /// <param name="count">the number of colors to be written</param>
        public void Write(Color[] pixels, int count)
        {
            byte[] barr = new byte[count * 3];
            for (int i = 0; i < count; i++)
            {
                // the order here is critical, it matches what the NeoPixels expect
                barr[i * 3 + 0] = ColorUtility.GetGValue(pixels[i]);
                barr[i * 3 + 1] = ColorUtility.GetRValue(pixels[i]);
                barr[i * 3 + 2] = ColorUtility.GetBValue(pixels[i]);
            }
            NeoPixelNative.Write(barr, count, (UInt32)pin);

            /* // this is what the code would've looked like if I didn't care about RAM memory so much
            NeoPixel[] wsPixels = new NeoPixel[count];
            for (int i = 0; i < count; i++)
            {
                wsPixels[i] = new NeoPixel(pixels[i]);
            }
            Write(wsPixels, count);
            //*/
        }

        /// <summary>
        /// The "Color" enum is a used for generic graphic stuff inside Microsoft.SPOT
        /// This method allows you to set NeoPixels using Microsoft.SPOT's "Color"
        /// Note: the "Color" enum is a 32 bit integer but GRB is 24 bit, however there are no data types available for just 24 bits
        /// Note: the count is derived from the array properties
        /// </summary>
        /// <param name="pixels">a list of colors to be written, index 0 is the first NeoPixel connected to the MCU</param>
        public void Write(Color[] pixels)
        {
            Write(pixels, pixels.Length);
        }

        /// <summary>
        /// Updates a set of NeoPixels
        /// 
        /// Note: the count is derived from the array properties
        /// </summary>
        /// <param name="pixels">a list of NeoPixels to update, index 0 is the first NeoPixel connected to the MCU</param>
        public void Write(NeoPixel[] pixels)
        {
            Write(pixels, pixels.Length);
        }

        #region Bitmap Writing Utilities

        /// <summary>
        /// "delegates" are used to describe "function pointers"
        /// Functions designed for this delegate is used so the user can describe how a grid of NeoPixels are arranged
        /// For generic arrangements, try using "GridArrangement" instead
        /// </summary>
        /// <param name="x">x coordinate, 0 is left</param>
        /// <param name="y">y coordinate, 0 is top</param>
        /// <param name="w">width of the bitmap</param>
        /// <param name="h">height of the bitmap</param>
        /// <returns>index of NeoPixel in chain</returns>
        public delegate int GetIndexDelegate(int x, int y, int w, int h);

        /// <summary>
        /// Draws an entire bitmap to a grid of NeoPixels
        /// </summary>
        /// <param name="bm">the bitmap to draw</param>
        /// <param name="GetIndexFunc">a "function pointer" so the user can specify how the grid is arranged. for generic arrangements, try using "GridArrangement" instead</param>
        public void Write(Bitmap bm, GetIndexDelegate GetIndexFunc)
        {
            // note: top left corner of a bitmap is (0,0)

            int count = bm.Width * bm.Height;
            byte[] barr = new byte[count * 3];
            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    Color c = bm.GetPixel(x, y);
                    int idx = GetIndexFunc(x, y, bm.Width, bm.Height);
                    barr[idx * 3 + 0] = ColorUtility.GetGValue(c);
                    barr[idx * 3 + 1] = ColorUtility.GetRValue(c);
                    barr[idx * 3 + 2] = ColorUtility.GetBValue(c);
                }
            }
            NeoPixelNative.Write(barr, count, (UInt32)pin);

            /* // this is what the code would've looked like if I didn't care about RAM memory so much
            Color[] pix = new Color[bm.Width * bm.Height];
            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    Color c = bm.GetPixel(x, y);
                    int idx = GetIndexFunc(x, y, bm.Width, bm.Height);
                    pix[idx] = c;
                }
            }

            Write(pix);
            //*/
        }

        /// <summary>
        /// GridArrangment tells the bitmap writing routine how the user has arraged their NeoPixels in a grid
        /// 
        /// Note: (x, y) of (0, 0) is always top left corner of the grid
        /// 
        /// Comments of each enum value is a graphical representation if an example 5x5 grid using indices of pixels
        /// </summary>
        public enum GridArrangment
        {
            /// <summary>
            /// (00)(01)(02)(03)(04)
            /// (05)(06)(07)(08)(09)
            /// (10)(11)(12)(13)(14)
            /// (15)(16)(17)(18)(19)
            /// (20)(21)(22)(23)(24)
            /// </summary>
            LeftToRight_TopToBottom_Zigzag,

            /// <summary>
            /// (20)(21)(22)(23)(24)
            /// (15)(16)(17)(18)(19)
            /// (10)(11)(12)(13)(14)
            /// (05)(06)(07)(08)(09)
            /// (00)(01)(02)(03)(04)
            /// </summary>
            LeftToRight_BottomToTop_Zigzag,

            /// <summary>
            /// (04)(03)(02)(01)(00)
            /// (09)(08)(07)(06)(05)
            /// (14)(13)(12)(11)(10)
            /// (19)(18)(17)(16)(15)
            /// (24)(23)(22)(21)(20)
            /// </summary>
            RightToLeft_TopToBottom_Zigzag,

            /// <summary>
            /// (24)(23)(22)(21)(20)
            /// (19)(18)(17)(16)(15)
            /// (14)(13)(12)(11)(10)
            /// (09)(08)(07)(06)(05)
            /// (04)(03)(02)(01)(00)
            /// </summary>
            RightToLeft_BottomToTop_Zigzag,

            /// <summary>
            /// (00)(05)(10)(15)(20)
            /// (01)(06)(11)(16)(21)
            /// (02)(07)(12)(17)(22)
            /// (03)(08)(13)(18)(23)
            /// (04)(09)(14)(19)(24)
            /// </summary>
            TopToBottom_LeftToRight_Zigzag,

            /// <summary>
            /// (20)(15)(10)(05)(00)
            /// (21)(16)(11)(06)(01)
            /// (22)(17)(12)(07)(02)
            /// (23)(18)(13)(08)(03)
            /// (24)(19)(14)(09)(04)
            /// </summary>
            TopToBottom_RightToLeft_Zigzag,

            /// <summary>
            /// (04)(09)(14)(19)(24)
            /// (03)(08)(13)(18)(23)
            /// (02)(07)(12)(17)(22)
            /// (01)(06)(11)(16)(21)
            /// (00)(05)(10)(15)(20)
            /// </summary>
            BottomToTop_LeftToRight_Zigzag,

            /// <summary>
            /// (20)(15)(10)(05)(00)
            /// (21)(16)(11)(06)(01)
            /// (22)(17)(12)(07)(02)
            /// (23)(18)(13)(08)(03)
            /// (24)(19)(14)(09)(04)
            /// </summary>
            BottomToTop_RightToLeft_Zigzag,

            /// <summary>
            /// (00)(01)(02)(03)(04)
            /// (09)(08)(07)(06)(05)
            /// (10)(11)(12)(13)(14)
            /// (19)(18)(17)(16)(15)
            /// (20)(21)(22)(23)(24)
            /// </summary>
            LeftToRight_TopToBottom_Snake,

            /// <summary>
            /// (20)(21)(22)(23)(24)
            /// (19)(18)(17)(16)(15)
            /// (10)(11)(12)(13)(14)
            /// (09)(08)(07)(06)(05)
            /// (00)(01)(02)(03)(04)
            /// </summary>
            LeftToRight_BottomToTop_Snake,

            /// <summary>
            /// (04)(03)(02)(01)(00)
            /// (05)(06)(07)(08)(09)
            /// (14)(13)(12)(11)(10)
            /// (15)(16)(17)(18)(19)
            /// (24)(23)(22)(21)(20)
            /// </summary>
            RightToLeft_TopToBottom_Snake,

            /// <summary>
            /// (24)(23)(22)(21)(20)
            /// (15)(16)(17)(18)(19)
            /// (14)(13)(12)(11)(10)
            /// (05)(06)(07)(08)(09)
            /// (04)(03)(02)(01)(00)
            /// </summary>
            RightToLeft_BottomToTop_Snake,

            /// <summary>
            /// (00)(09)(10)(19)(20)
            /// (01)(08)(11)(18)(21)
            /// (02)(07)(12)(17)(22)
            /// (03)(06)(13)(16)(23)
            /// (04)(05)(14)(15)(24)
            /// </summary>
            TopToBottom_LeftToRight_Snake,

            /// <summary>
            /// (20)(19)(10)(09)(00)
            /// (21)(18)(11)(08)(01)
            /// (22)(17)(12)(07)(02)
            /// (23)(16)(13)(06)(03)
            /// (24)(15)(14)(05)(04)
            /// </summary>
            TopToBottom_RightToLeft_Snake,

            /// <summary>
            /// (04)(05)(14)(15)(24)
            /// (03)(06)(13)(16)(23)
            /// (02)(07)(12)(17)(22)
            /// (01)(08)(11)(18)(21)
            /// (00)(09)(10)(19)(20)
            /// </summary>
            BottomToTop_LeftToRight_Snake,

            /// <summary>
            /// (24)(15)(14)(05)(04)
            /// (23)(16)(13)(06)(03)
            /// (22)(17)(12)(07)(02)
            /// (21)(18)(11)(08)(01)
            /// (20)(19)(10)(09)(00)
            /// </summary>
            BottomToTop_RightToLeft_Snake,
        }

        /// <summary>
        /// used by GetIndexFromXYAndGridArrangement only
        /// </summary>
        private GridArrangment usedGridArrange;

        /// <summary>
        /// calculates the appropriate chain index of a pixel
        /// using x and y coordinates in combination of a generic grid arrangement
        /// </summary>
        /// <param name="x">x coordinate, 0 is left</param>
        /// <param name="y">y coordinate, 0 is top</param>
        /// <param name="w">width of the bitmap</param>
        /// <param name="h">height of the bitmap</param>
        /// <returns>index of NeoPixel in chain</returns>
        private int GetIndexFromXYAndGridArrangement(int x, int y, int w, int h)
        {
            switch (usedGridArrange)
            {
                case GridArrangment.LeftToRight_TopToBottom_Zigzag:
                    return y * w + x;                    
                case GridArrangment.LeftToRight_BottomToTop_Zigzag:
                    return (h - y - 1) * w + x;                    
                case GridArrangment.RightToLeft_TopToBottom_Zigzag:
                    return y * w + (w - x - 1);                    
                case GridArrangment.RightToLeft_BottomToTop_Zigzag:
                    return (h - y - 1) * w + (w - x - 1);                    
                case GridArrangment.TopToBottom_LeftToRight_Zigzag:
                    return x * h + y;                    
                case GridArrangment.TopToBottom_RightToLeft_Zigzag:
                    return (w - x - 1) * h + y;                    
                case GridArrangment.BottomToTop_LeftToRight_Zigzag:
                    return x * h + (h - y - 1);                    
                case GridArrangment.BottomToTop_RightToLeft_Zigzag:
                    return (w - x - 1) * h + (h - y - 1);                    
                case GridArrangment.LeftToRight_TopToBottom_Snake:
                    return (y % 2 == 0) ? (y * w + x) : ((h - y - 1) * w + x);                    
                case GridArrangment.LeftToRight_BottomToTop_Snake:
                    return (y % 2 == 0) ? ((h - y - 1) * w + x) : (y * w + x);                    
                case GridArrangment.RightToLeft_TopToBottom_Snake:
                    return (y % 2 == 0) ? (y * w + (w - x - 1)) : ((h - y - 1) * w + (w - x - 1));                    
                case GridArrangment.RightToLeft_BottomToTop_Snake:
                    return (y % 2 == 0) ? ((h - y - 1) * w + (w - x - 1)) : (y * w + (w - x - 1));                    
                case GridArrangment.TopToBottom_LeftToRight_Snake:
                    return (x % 2 == 0) ? (x * h + y) : ((w - x - 1) * h + y);                    
                case GridArrangment.TopToBottom_RightToLeft_Snake:
                    return (x % 2 == 0) ? ((w - x - 1) * h + y) : (x * h + y);                    
                case GridArrangment.BottomToTop_LeftToRight_Snake:
                    return (x % 2 == 0) ? (x * h + (h - y - 1)) : ((w - x - 1) * h + (h - y - 1));                    
                case GridArrangment.BottomToTop_RightToLeft_Snake:
                    return (x % 2 == 0) ? ((w - x - 1) * h + (h - y - 1)) : (x * h + (h - y - 1));
            }
            throw new Exception("Invalid Grid Arrangement");
        }

        /// <summary>
        /// Draws an entire bitmap to a grid of NeoPixels
        /// </summary>
        /// <param name="bm">the bitmap to draw</param>
        /// <param name="gridArrange">how the NeoPixel grid is arranged</param>
        public void Write(Bitmap bm, GridArrangment gridArrange)
        {
            usedGridArrange = gridArrange; // used by GetIndexFromXYAndGridArrangement only
            Write(bm, new GetIndexDelegate(GetIndexFromXYAndGridArrangement));
        }
        #endregion
    }
}
