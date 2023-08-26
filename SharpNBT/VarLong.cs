using System;
using System.IO;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Provides static methods for reading and writing variable-length integers that are up to 10 bytes from both streams and buffers.
/// </summary>
[PublicAPI]
public static class VarLong
{
    /// <summary>
    /// Encodes the given <paramref name="value"/> to a variable-length integer up to 10 bytes long, and writes it to the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> instance to write the value to.</param>
    /// <param name="value">The value to encode and write.</param>
    /// <param name="zigzag">Flag indicating if the value will be ZigZag encoded.</param>
    /// <returns>The number of bytes written to the <paramref name="stream"/>.</returns>
    public static int Write(Stream stream, long value, bool zigzag = false)
    {
        var buffer = Encode(value, zigzag);
        stream.Write(buffer, 0, buffer.Length);
        return buffer.Length;
    }
        
    /// <summary>
    /// Reads up to 10 bytes from the given <paramref name="stream"/> and returns the VarLong value as a 64-bit integer.
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> instance to read from.</param>
    /// <param name="zigzag">Flag indicating if the value is ZigZag encoded.</param>
    /// <returns>The parsed value read from the <paramref name="stream"/>.</returns>
    public static long Read(Stream stream, bool zigzag = false)
    {
        var value = VarIntUtil.Decode(stream, 64, out var dummy);
        return zigzag ? VarIntUtil.DecodeZigZag(value) : unchecked((long) value);
    }

    /// <summary>
    /// Reads up to 10 bytes from the given <paramref name="stream"/> and returns the VarLong value as a 64-bit integer.
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> instance to read from.</param>
    /// <param name="size">A variable to store the number of bytes read from the <paramref name="stream"/>.</param>
    /// <param name="zigzag">Flag indicating if the value is ZigZag encoded.</param>
    /// <returns>The parsed value read from the <paramref name="stream"/>.</returns>
    public static long Read(Stream stream, out int size, bool zigzag = false)
    {
        var value = VarIntUtil.Decode(stream, 64, out size);
        return zigzag ? VarIntUtil.DecodeZigZag(value) : unchecked((long) value);
    }
        
    /// <summary>
    /// Encodes the given <paramref name="value"/> and returns an array of bytes that represent it.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <param name="zigzag">Flag indicating if the value will be ZigZag encoded.</param>
    /// <returns>An array of bytes representing the <paramref name="value"/> as a variable length integer.</returns>
    public static byte[] Encode(long value, bool zigzag = false)
    {
        if (zigzag)
            return VarIntUtil.Encode(unchecked((ulong)VarIntUtil.EncodeZigZag(value, 64)));
        return VarIntUtil.Encode(unchecked((ulong)value));
    }
        
    /// <summary>
    /// Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.
    /// </summary>
    /// <param name="buffer">A buffer containing the data to be decoded.</param>
    /// <param name="offset">The offset into the <paramref name="buffer"/> to begin reading.</param>
    /// <param name="count">The maximum number of bytes that should be read from the <paramref name="buffer"/>.</param>
    /// <param name="size">A variable to store the actual number of bytes read from the <paramref name="buffer"/>.</param>
    /// <param name="zigzag">Flag indicating if the value is ZigZag encoded.</param>
    /// <returns>The decoded value.</returns>
    public static long Decode(byte[] buffer, int offset, int count, out int size, bool zigzag = false)
    {
        return Decode(new ReadOnlySpan<byte>(buffer, offset, count), out size, zigzag);
    }
        
    /// <summary>
    /// Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.
    /// </summary>
    /// <param name="buffer">A buffer containing the data to be decoded.</param>
    /// <param name="offset">The offset into the <paramref name="buffer"/> to begin reading.</param>
    /// <param name="count">The maximum number of bytes that should be read from the <paramref name="buffer"/>.</param>
    /// <param name="zigzag">Flag indicating if the value is ZigZag encoded.</param>
    /// <returns>The decoded value.</returns>
    public static long Decode(byte[] buffer, int offset, int count, bool zigzag = false)
    {
        return Decode(new ReadOnlySpan<byte>(buffer, offset, count), out var dummy, zigzag);
    }
        
    /// <summary>
    /// Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.
    /// </summary>
    /// <param name="buffer">A buffer containing the data to be decoded.</param>
    /// <param name="size">A variable to store the actual number of bytes read from the <paramref name="buffer"/>.</param>
    /// <param name="zigzag">Flag indicating if the value is ZigZag encoded.</param>
    /// <returns>The decoded value.</returns>
    public static long Decode(ReadOnlySpan<byte> buffer, out int size, bool zigzag = false)
    {
        var value = VarIntUtil.Decode(buffer, 64, out size);
        return zigzag ? VarIntUtil.DecodeZigZag(value) : unchecked((long)value);
    }
        
    /// <summary>
    /// Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.
    /// </summary>
    /// <param name="buffer">A buffer containing the data to be decoded.</param>
    /// <param name="zigzag">Flag indicating if the value is ZigZag encoded.</param>
    /// <returns>The decoded value.</returns>
    public static long Decode(ReadOnlySpan<byte> buffer, bool zigzag = false)
    {
        var value = VarIntUtil.Decode(buffer, 64, out var dummy);
        return zigzag ? VarIntUtil.DecodeZigZag(value) : unchecked((long)value);
    }
}