using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation.Media;

namespace NeoPixel
{
    /// <summary>
    /// This class helps the user organize their set of NeoPixels,
    /// because NeoPixelChain pretends to be a list, and have all of the
    /// similar properties and methods associated with a simple list.
    /// The user can work with their NeoPixels as though it was continous chain.
    /// 
    /// Unlike NeoPixelWriter (which contains multiple complex Write methods), NeoPixelChain only has one simple Write method that Writes the entire chain
    /// </summary>
    public class NeoPixelChain : IList
    {
        private Cpu.Pin pin;
        private OutputPort op;
        private ArrayList pixels = new ArrayList();

        /// <summary>
        /// accesses a particular NeoPixel in the chain
        /// index 0 is the first NeoPixel connected to the MCU
        /// </summary>
        public NeoPixel this[int index]
        {
            get
            {
                return (NeoPixel)pixels[index];
            }

            set
            {
                pixels[index] = value;
            }
        }

        /// <summary>
        /// Constructor that initializes a single pin that is connected to the first NeoPixel's data input
        /// </summary>
        /// <param name="pin">MCU pin that is connected to the first NeoPixel's data input</param>
        public NeoPixelChain(Cpu.Pin pin)
        {
            this.pin = pin;
            op = new OutputPort(pin, false);
        }

        /// <summary>
        /// Similar to the other constructor assumes that the pin is pre-initialized
        /// </summary>
        /// <param name="pin">MCU pin that is connected to the first NeoPixel's data input, must be pre-initialized or initialized before "Write"</param>
        public NeoPixelChain(OutputPort pin)
        {
            op = pin;
            this.pin = op.Id;
        }

        /// <summary>
        /// Updates every NeoPixel in this chain
        /// </summary>
        public void Write()
        {
            byte[] barr = new byte[pixels.Count * 3];
            for (int i = 0; i < pixels.Count; i++)
            {
                // the order here is critical, it matches what the NeoPixels expect
                barr[i * 3 + 0] = this[i].Green;
                barr[i * 3 + 1] = this[i].Red;
                barr[i * 3 + 2] = this[i].Blue;
            }
            NeoPixelNative.Write(barr, pixels.Count, (UInt32)pin);
        }

        /// <summary>
        /// Adds a default NeoPixel to the chain
        /// </summary>
        /// <returns>reference to the NeoPixel that was added</returns>
        public NeoPixel Add()
        {
            int i = this.Add(new NeoPixel());
            if (i >= 0)
            {
                return this[i];
            }
            throw new Exception("Add Failed");
        }

        /// <summary>
        /// Adds a NeoPixel to the chain with a specific color
        /// </summary>
        /// <param name="c">color of the NeoPixel to be added</param>
        /// <returns>reference to the NeoPixel that was added</returns>
        public NeoPixel Add(Color c)
        {
            int i = this.Add(new NeoPixel(c));
            if (i >= 0)
            {
                return this[i];
            }
            throw new Exception("Add Failed");
        }

        /// <summary>
        /// Adds a NeoPixel to the chain with a specific color
        /// </summary>
        /// <param name="r">red value of the NeoPixel to be added, 0 to 255</param>
        /// <param name="g">green value of the NeoPixel to be added, 0 to 255</param>
        /// <param name="b">blue value of the NeoPixel to be added, 0 to 255</param>
        /// <returns>reference to the NeoPixel that was added</returns>
        public NeoPixel Add(byte r, byte g, byte b)
        {
            int i = this.Add(new NeoPixel(r, g, b));
            if (i >= 0)
            {
                return this[i];
            }
            throw new Exception("Add Failed");
        }

        /// <summary>
        /// Adds a NeoPixel to the chain with a specific color
        /// </summary>
        /// <param name="r">red value of the NeoPixel to be added, 0 to 255</param>
        /// <param name="g">green value of the NeoPixel to be added, 0 to 255</param>
        /// <param name="b">blue value of the NeoPixel to be added, 0 to 255</param>
        /// <returns>reference to the NeoPixel that was added</returns>
        public NeoPixel Add(int r, int g, int b)
        {
            return Add((byte)r, (byte)g, (byte)b);
        }

        #region IList Members
        /// <summary>
        /// Adds an object to the chain
        /// 
        /// Warning: object must be a NeoPixel instance
        /// </summary>
        /// <param name="value">object to be added, must be a NeoPixel instance</param>
        /// <returns>index of where the object was added in the chain</returns>
        public int Add(object value)
        {
            int i = pixels.Add(value);
            if (i < 0)
                throw new Exception("Add Failed");
            return i;
        }

        /// <summary>
        /// Adds a NeoPixel to the chain
        /// </summary>
        /// <param name="px">NeoPixel to be added</param>
        /// <returns>index of where the NeoPixel was added to</returns>
        public int Add(NeoPixel px)
        {
            int i = pixels.Add(px);
            if (i < 0)
                throw new Exception("Add Failed");
            return i;
        }

        /// <summary>
        /// Removes all NeoPixels from chain
        /// </summary>
        public void Clear()
        {
            pixels.Clear();
        }

        /// <summary>
        /// Whether or not the chain contains a reference to an object
        /// 
        /// Note: this is only a search by reference
        /// </summary>
        /// <param name="value">object to check</param>
        /// <returns>whether or not the chain contains a reference to an object</returns>
        public bool Contains(object value)
        {
            return pixels.Contains(value);
        }

        /// <summary>
        /// Whether or not the chain contains a reference to a NeoPixel instance
        /// 
        /// Note: this is only a search by reference
        /// </summary>
        /// <param name="px">NeoPixel instance to check</param>
        /// <returns>whether or not the chain contains a reference to a NeoPixel instance</returns>
        public bool Contains(NeoPixel px)
        {
            return pixels.Contains(px);
        }

        /// <summary>
        /// Find where a specific object is in the chain
        /// 
        /// Note: this is only a search by reference
        /// </summary>
        /// <param name="value">object to look for</param>
        /// <returns>index of the object in the chain, or negative if it is not found</returns>
        public int IndexOf(object value)
        {
            return pixels.IndexOf(value);
        }

        /// <summary>
        /// Find where a specific NeoPixel instance is in the chain
        /// 
        /// Note: this is only a search by reference
        /// </summary>
        /// <param name="px">NeoPixel instance to look for</param>
        /// <returns>index of the NeoPixel instance in the chain, or negative if it is not found</returns>
        public int IndexOf(NeoPixel px)
        {
            return pixels.IndexOf(px);
        }

        /// <summary>
        /// Inserts an object into the chain at a specific index.
        /// 
        /// Warning: object must be a NeoPixel instance
        /// </summary>
        /// <param name="index">index of the insertion</param>
        /// <param name="value">object to be inserted</param>
        public void Insert(int index, object value)
        {
            pixels.Insert(index, value);
        }

        /// <summary>
        /// Inserts a NeoPixel into the chain at a specific index.
        /// </summary>
        /// <param name="index">index of the insertion</param>
        /// <param name="px">NeoPixel to be inserted</param>
        public void Insert(int index, NeoPixel px)
        {
            pixels.Insert(index, px);
        }

        /// <summary>
        /// Whether or not the chain has a fixed size
        /// 
        /// Note: the chain should always be dynamically sized
        /// </summary>
        public bool IsFixedSize
        {
            get { return pixels.IsFixedSize; }
        }

        /// <summary>
        /// Whether or not the chain is read-only
        /// 
        /// Note: the chain should not be read-only
        /// </summary>
        public bool IsReadOnly
        {
            get { return pixels.IsReadOnly; }
        }

        /// <summary>
        /// Removes an object from the chain
        /// 
        /// Note: this only searches by reference
        /// </summary>
        /// <param name="value">object to be removed</param>
        public void Remove(object value)
        {
            pixels.Remove(value);
        }

        /// <summary>
        /// Removes a NeoPixel instance from the chain
        /// 
        /// Note: this only searches by reference
        /// </summary>
        /// <param name="px">NeoPixel instance to be removed</param>
        public void Remove(NeoPixel px)
        {
            pixels.Remove(px);
        }

        /// <summary>
        /// Removes a NeoPixel at a specific index in the chain
        /// </summary>
        /// <param name="index">index of the NeoPixel to be removed</param>
        public void RemoveAt(int index)
        {
            pixels.RemoveAt(index);
        }

        /// <summary>
        /// Object at a specific index in the chain
        /// </summary>
        /// <param name="index">index of object</param>
        /// <returns>object at the index</returns>
        object IList.this[int index]
        {
            get
            {
                return pixels[index];
            }
            set
            {
                pixels[index] = value;
            }
        }

        /// <summary>
        /// Copies the elements of the chain to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">destination Array</param>
        /// <param name="index">starting index</param>
        public void CopyTo(Array array, int index)
        {
            pixels.CopyTo(array, index);
        }

        /// <summary>
        /// How many NeoPixels are in this chain
        /// </summary>
        public int Count
        {
            get { return pixels.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the chain is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return pixels.IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the chain
        /// </summary>
        public object SyncRoot
        {
            get { return pixels.SyncRoot; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the chain
        /// </summary>
        /// <returns>enumerator that iterates through the chain</returns>
        public IEnumerator GetEnumerator()
        {
            return pixels.GetEnumerator();
        }
        #endregion
    }
}
