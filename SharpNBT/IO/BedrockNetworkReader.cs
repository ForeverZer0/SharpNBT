using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpNBT.IO;

/// <summary>
/// A NBT reader for the Minecraft Bedrock <b>network</b>. This is not compatible with Minecraft Bedrock NBT that was
/// read from a file stream, which uses a different format.
/// <para/>
/// All input data is natively in little-endian format. Integer-types are usually a variable length integer, and not
/// a fixed width. Sometimes they are also ZigZag-encoded like Protobuf. Also sometimes they are not.
/// <para/>
/// Thank you, Microsoft.
/// </summary>
public class BedrockNetworkReader : TagReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BedrockNetworkReader"/> class.
    /// </summary>
    /// <param name="stream">A stream containing the NBT data.</param>
    /// <param name="encoding">
    /// The text-encoding for strings, or <see langword="null"/> to use the default for the format.
    /// </param>
    public BedrockNetworkReader(Stream stream, Encoding? encoding = null) : base(stream, encoding ?? Encoding.UTF8)
    {
    }
    
    /// <summary>
    /// Reads a variable-length integer from the stream, with optional ZigZag-encoding.
    /// </summary>
    /// <param name="bits">The bit-width of the target integer type.</param>
    /// <param name="zigzag">
    /// <see langword="true"/> to indicate the value is ZigZag-encoded, otherwise <see langword="false"/>.
    /// </param>
    /// <returns>The decoded integer value.</returns>
    /// <exception cref="OverflowException">
    /// The value cannot be decoded into an integer with the specified number of <paramref name="bits"/>.
    /// </exception>
    private long ReadVarInt(int bits, bool zigzag)
    {
        var shift = 0;
        ulong result = 0;
        
        while (true)
        {
            var b = (ulong) BaseStream.ReadByte();
            var tmp = b & 0x7F;
            result |= tmp << shift;

            if (shift > bits)
                throw new OverflowException($"Variable length integer cannot be decoded into {bits} bits.");

            if ((b & 0x80) != 0x80)
            {
                if (!zigzag)
                    return Unsafe.As<ulong, long>(ref result);
                
                if ((result & 0x1) == 0x1)
                    return -1 * unchecked((long)(result >> 1) + 1);
                return unchecked((long)(result >> 1));
            }

            shift += 7;
        }
    }
    
    /// <summary>
    /// Reads a string from the stream. Assumes that the string is length-prefixed with a variable-length integer.
    /// </summary>
    /// <returns>The string value.</returns>
    private string ReadString()
    {
        var length = unchecked((int) ReadVarInt(32, false));
        return ReadString(length);
    }

    /// <inheritdoc />
    protected override ByteTag ReadByte(bool named)
    {
        var name = named ? ReadString() : null;
        return new ByteTag(name, BaseStream.ReadByte());
    }

    /// <inheritdoc />
    protected override ShortTag ReadShort(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        BaseStream.ReadExactly(buffer);
        return new ShortTag(name, BinaryPrimitives.ReadInt16LittleEndian(buffer));
    }

    /// <inheritdoc />
    protected override IntTag ReadInt(bool named)
    {
        var name = named ? ReadString() : null;
        return new IntTag(name, (int) ReadVarInt(32, true));
    }

    /// <inheritdoc />
    protected override LongTag ReadLong(bool named)
    {
        var name = named ? ReadString() : null;
        return new LongTag(name, ReadVarInt(64, true));
    }

    /// <inheritdoc />
    protected override FloatTag ReadFloat(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BaseStream.ReadExactly(buffer);
        return new FloatTag(name, BinaryPrimitives.ReadSingleLittleEndian(buffer));
    }

    /// <inheritdoc />
    protected override DoubleTag ReadDouble(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BaseStream.ReadExactly(buffer);
        return new DoubleTag(name, BinaryPrimitives.ReadDoubleLittleEndian(buffer));
    }

    /// <inheritdoc />
    protected override StringTag ReadString(bool named)
    {
        var name = named ? ReadString() : null;
        return new StringTag(name, ReadString());
    }

    /// <inheritdoc />
    protected override ByteArrayTag ReadByteArray(bool named)
    {
        byte[] values;
        var name = named ? ReadString() : null;
        var length = (int)ReadVarInt(32, true);
        if (length == 0)
        {
            values = Array.Empty<byte>();
        }
        else
        {
            values = new byte[length];
            BaseStream.ReadExactly(values);
        }
        return new ByteArrayTag(name, values);
    }

    /// <inheritdoc />
    protected override IntArrayTag ReadIntArray(bool named)
    {
        var name = named ? ReadString() : null;
        var length = (int)ReadVarInt(32, true);
        if (length == 0)
            return new IntArrayTag(name, Array.Empty<int>());

        var values = new int[length];
        BaseStream.ReadExactly(MemoryMarshal.Cast<int, byte>(values));

        if (BitConverter.IsLittleEndian) 
            return new IntArrayTag(name, values);
        
        for (var i = 0; i < length; i++)
            values[i] = values[i].SwapEndian();
        return new IntArrayTag(name, values);
    }

    /// <inheritdoc />
    protected override LongArrayTag ReadLongArray(bool named)
    {
        var name = named ? ReadString() : null;
        var length = (int)ReadVarInt(32, true);
        if (length == 0)
            return new LongArrayTag(name, Array.Empty<long>());

        var values = new long[length];
        BaseStream.ReadExactly(MemoryMarshal.Cast<long, byte>(values));

        if (BitConverter.IsLittleEndian) 
            return new LongArrayTag(name, values);
        
        for (var i = 0; i < length; i++)
            values[i] = values[i].SwapEndian();
        return new LongArrayTag(name, values);
    }

    /// <inheritdoc />
    protected override ListTag ReadList(bool named)
    {
        var name = named ? ReadString() : null;
        var childType = (TagType)BaseStream.ReadByte();
        var length = (int)ReadVarInt(32, true);

        var list = new ListTag(name);
        for (var i = 0; i < length; i++)
            list.Add(Read(false, childType));

        return list;
    }

    /// <inheritdoc />
    protected override CompoundTag ReadCompound(bool named)
    {
        var name = named ? ReadString() : null;
        var compound = new CompoundTag(name);
        
        while (true)
        {
            var child = Read(true);
            if (child is EndTag)
                break;
            compound.Add(child);
        }
        
        return compound;
    }
}