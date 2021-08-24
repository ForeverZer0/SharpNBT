using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Contains extension methods dealing with endianness of numeric types.
    /// </summary>
    [PublicAPI]
    public static class EndianExtensions
    {
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
        public static ulong SwapEndian(this ulong value)
        {
            value = ((value << 8)  & 0xFF00FF00FF00FF00UL) | ((value >> 8)  & 0x00FF00FF00FF00FFUL);
            value = ((value << 16) & 0xFFFF0000FFFF0000UL) | ((value >> 16) & 0x0000FFFF0000FFFFUL);
            return (value << 32) | (value >> 32);
        }
        
        /// <inheritdoc cref="SwapEndian(short)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SwapEndian(this long value) => unchecked((long) SwapEndian(unchecked((ulong)value)));

        /// <inheritdoc cref="SwapEndian(short)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SwapEndian(this float value)
        {
            var n = BitConverter.SingleToInt32Bits(value);
            return BitConverter.Int32BitsToSingle(n.SwapEndian());
        }
        
        /// <inheritdoc cref="SwapEndian(short)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SwapEndian(this double value)
        {
            var n = BitConverter.DoubleToInt64Bits(value);
            return BitConverter.Int64BitsToDouble(n.SwapEndian());
        }
    }
}