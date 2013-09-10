using System;
using Microsoft.SPOT.Presentation.Media;

namespace NeoPixel
{
    /// <summary>
    /// This class simply contains the color info of one NeoPixel
    /// with some helpful constructors and properties
    /// </summary>
    public class NeoPixel
    {
        /// <summary>
        /// Green intensity of this NeoPixel, 0 to 255
        /// </summary>
        public byte Green
        {
            get;
            set;
        }

        /// <summary>
        /// Red intensity of this NeoPixel, 0 to 255
        /// </summary>
        public byte Red
        {
            get;
            set;
        }

        /// <summary>
        /// Blue intensity of this NeoPixel, 0 to 255
        /// </summary>
        public byte Blue
        {
            get;
            set;
        }

        /// <summary>
        /// Color (Microsoft.SPOT.Presentation.Media) of this NeoPixel
        /// </summary>
        public Color Color
        {
            get
            {
                return ColorUtility.ColorFromRGB(this.Red, this.Green, this.Blue);
            }

            set
            {
                this.Green = ColorUtility.GetGValue(value);
                this.Red = ColorUtility.GetRValue(value);
                this.Blue = ColorUtility.GetBValue(value);
            }
        }

        /// <summary>
        /// default constructor, default color is "darkest" (off)
        /// </summary>
        public NeoPixel() : this((byte)0)
        {
        }

        /// <summary>
        /// constructor that sets a initial color
        /// </summary>
        /// <param name="c">initial color</param>
        public NeoPixel(Color c)
        {
            this.Green = ColorUtility.GetGValue(c);
            this.Red = ColorUtility.GetRValue(c);
            this.Blue = ColorUtility.GetBValue(c);
        }

        /// <summary>
        /// constructor that sets an initial "gray" brightness
        /// </summary>
        /// <param name="brightness">initial "gray" brightness, 0 to 255</param>
        public NeoPixel(byte brightness)
        {
            this.Green = brightness;
            this.Red = brightness;
            this.Blue = brightness;
        }

        /// <summary>
        /// constructor that sets an initial color, specified as RGB
        /// </summary>
        /// <param name="r">initial red level, 0 to 255</param>
        /// <param name="g">initial green level, 0 to 255</param>
        /// <param name="b">initial blue level, 0 to 255</param>
        public NeoPixel(byte r, byte g, byte b)
        {
            this.Green = g;
            this.Red = r;
            this.Blue = b;
        }

        /// <summary>
        /// constructor that sets an initial color, specified as RGB
        /// </summary>
        /// <param name="r">initial red level, 0 to 255</param>
        /// <param name="g">initial green level, 0 to 255</param>
        /// <param name="b">initial blue level, 0 to 255</param>
        public NeoPixel(int r, int g, int b)
        {
            this.Green = (byte)g;
            this.Red = (byte)r;
            this.Blue = (byte)b;
        }
    }
}