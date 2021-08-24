using System;
using System.Runtime.CompilerServices;

namespace SharpNBT.ZLib
{
    /// <summary>
    /// An Adler-32 checksum implementation for ZLib streams.
    /// </summary>
    /// <seealso href=""/>
    public sealed class Adler32
    {
        private uint a = 1;
        private uint b;
        private const int BASE = 65521;
        private const int MAX = 5550;
        private int pending;

        /// <summary>
        /// Update the checksum value with the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">A single value to calculate into the checksum.</param>
        public void Update(byte data) 
        {
            if (pending >= MAX) 
                UpdateModulus();
            a += data;
            b += a;
            pending++;
        }

        /// <summary>
        /// Update the checksum value with the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">A buffer containing the values to calculate into the checksum.</param>
        public void Update(byte[] data) => Update(new ReadOnlySpan<byte>(data, 0, data.Length));

        /// <summary>
        /// Update the checksum value with the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">A buffer containing the values to calculate into the checksum.</param>
        public void Update(ReadOnlySpan<byte> data)
        {
            unchecked
            {
                var nextCompute = MAX - pending;
                for (var i = 0; i < data.Length; i++) 
                {
                    if (i == nextCompute) 
                    {
                        UpdateModulus();
                        nextCompute = i + MAX;
                    }
                    a += data[i];
                    b += a;
                    pending++;
                }
            }
        }

        /// <summary>
        /// Update the checksum value with the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">A buffer containing the values to calculate into the checksum.</param>
        /// <param name="offset">An offset into the <paramref name="data"/> to begin adding from.</param>
        /// <param name="length">The number of bytes in <paramref name="data"/> to calculate.</param>
        public void Update(byte[] data, int offset, int length) => Update(new ReadOnlySpan<byte>(data, offset, length));
        
        /// <summary>
        /// Reset the checksum back to the initial state.
        /// </summary>
        public void Reset() 
        {
            a = 1;
            b = 0;
            pending = 0;
        }

        /// <summary>
        /// Gets the current calculated checksum value as a signed 32-bit integer.
        /// </summary>
        public int Value
        {
            get
            {
                if (pending > 0) 
                    UpdateModulus();
                return unchecked((int)((b << 16) | a));
            }
        }
        
        /// <summary>
        /// Gets the current calculated checksum value as an unsigned 32-bit integer.
        /// </summary>
        [CLSCompliant(false)]
        public uint UnsignedValue
        {
            get
            {
                if (pending > 0)
                    UpdateModulus();
                return (b << 16) | a;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateModulus()
        {
            a %= BASE;
            b %= BASE;
            pending = 0;
        }
    }
}