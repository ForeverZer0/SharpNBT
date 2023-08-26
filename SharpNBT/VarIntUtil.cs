using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SharpNBT;

internal static class VarIntUtil
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long EncodeZigZag(long value, int bitLength)
    {
        return (value << 1) ^ (value >> (bitLength - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DecodeZigZag(ulong value)
    {
        if ((value & 0x1) == 0x1)
        {
            return (-1 * ((long)(value >> 1) + 1));
        }
        return (long)(value >> 1);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Encode(uint value)
    {
        Span<byte> buffer = stackalloc byte[5];
        var pos = 0;
        do
        {
            var byteVal = value & 0x7f;
            value >>= 7;
            if (value != 0)
                byteVal |= 0x80;
            buffer[pos++] = (byte) byteVal;

        } while (value != 0);

        return buffer[..pos].ToArray();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Encode(ulong value)
    {
        Span<byte> buffer = stackalloc byte[10];
        var pos = 0;
        do
        {
            var byteVal = value & 0x7f;
            value >>= 7;
            if (value != 0)
                byteVal |= 0x80;
            buffer[pos++] = (byte) byteVal;

        } while (value != 0);

        return buffer[..pos].ToArray();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Decode(ReadOnlySpan<byte> buffer, int bits, out int size)
    {
        var shift = 0;
        ulong result = 0;
        size = 0;

        foreach (ulong byteValue in buffer)
        {
            ulong tmp = byteValue & 0x7f;
            result |= tmp << shift;
            if (shift > bits)
                throw new OverflowException(string.Format(Strings.VarIntTooMuchData, bits));
            size++;
            if ((byteValue & 0x80) != 0x80)
                return result;
                
            shift += 7;
        }
        throw new FormatException(Strings.VarIntCannotDecode);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Decode(Stream stream, int bits, out int size)
    {
        var shift = 0;
        ulong result = 0;
        size = 0;

        while (true)
        {
            var byteValue = (ulong) stream.ReadByte();
            size++;
                
            ulong tmp = byteValue & 0x7f;
            result |= tmp << shift;
            if (shift > bits)
                throw new OverflowException(string.Format(Strings.VarIntTooMuchData, bits));
            if ((byteValue & 0x80) != 0x80)
                return result;
                
            shift += 7;
        }
    }
}