using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Contains extension methods dealing with endianness of numeric types.
    /// </summary>
    [PublicAPI]
    public static class EndianUtils
    {
        /// <summary>
        /// Gets the big-endian representation of this number as an array of bytes.
        /// </summary>
        /// <param name="n">The value to convert.</param>
        /// <returns>An array of bytes representing the value in big-endian format.</returns>
        /// <remarks>The endianness of the host machine is accounted for.</remarks>
        public static byte[] BigEndianBytes(this short n) => BitConverter.GetBytes(BitConverter.IsLittleEndian ? SwapEndian(n) : n);

        /// <inheritdoc cref="BigEndianBytes(short)"/>
        public static byte[] BigEndianBytes(this int n) => BitConverter.GetBytes(BitConverter.IsLittleEndian ? SwapEndian(n) : n);
        
        /// <inheritdoc cref="BigEndianBytes(short)"/>
        public static byte[] BigEndianBytes(this long n) => BitConverter.GetBytes(BitConverter.IsLittleEndian ? SwapEndian(n) : n);
        
        /// <inheritdoc cref="BigEndianBytes(short)"/>
        [CLSCompliant(false)]
        internal static byte[] BigEndianBytes(this ushort n) => BitConverter.GetBytes(BitConverter.IsLittleEndian ? SwapEndian(n) : n);

        /// <inheritdoc cref="BigEndianBytes(short)"/>
        internal static byte[] BigEndianBytes(this float n)
        {
            var bytes = BitConverter.GetBytes(n);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <inheritdoc cref="BigEndianBytes"/>
        internal static byte[] BigEndianBytes(this double n)
        {
            var bytes = BitConverter.GetBytes(n);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Swap the endian of the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to swap endian of.</param>
        /// <returns>The value with bytes in opposite format.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SwapEndian(this short value)
        {
            return (short) ((value << 8) | ((value >> 8) & 0xFF));
        }
        
        /// <inheritdoc cref="SwapEndian(short)"/>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SwapEndian(this ushort value)
        {
            return (ushort)((value << 8) | (value >> 8 ));
        }

        /// <inheritdoc cref="SwapEndian(short)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SwapEndian(this int value) => unchecked((int) SwapEndian(unchecked((uint)value)));

        /// <inheritdoc cref="SwapEndian(short)"/>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SwapEndian(this uint value) 
        {
            value = ((value << 8) & 0xFF00FF00 ) | ((value >> 8) & 0xFF00FF ); 
            return (value << 16) | (value >> 16);
        }

        /// <inheritdoc cref="SwapEndian(short)"/>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SwapEndian(this ulong val)
        {
            val = ((val << 8) & 0xFF00FF00FF00FF00UL ) | ((val >> 8) & 0x00FF00FF00FF00FFUL );
            val = ((val << 16) & 0xFFFF0000FFFF0000UL ) | ((val >> 16) & 0x0000FFFF0000FFFFUL );
            return (val << 32) | (val >> 32);
        }
        
        /// <inheritdoc cref="SwapEndian(short)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SwapEndian(this long value) => unchecked((long) SwapEndian(unchecked((ulong)value)));
        
    }
}