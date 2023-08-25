using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpNBT.IO;

/// <summary>
/// A NBT reader for the Minecraft Bedrock <b>files</b>. This is not compatible with Minecraft Bedrock NBT that was
/// sent over a network stream, which uses a different format.
/// <para/>
/// All input data is natively in little-endian format, with no variable length integers. Aside from the endianness
/// difference, this is identical to the Java version.
/// </summary>
public class BedrockTagReader : TagReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BedrockTagReader"/> class.
    /// </summary>
    /// <param name="stream">A stream containing the NBT data.</param>
    /// <param name="encoding">
    /// The text-encoding for strings, or <see langword="null"/> to use the default for the format.
    /// </param>
    public BedrockTagReader(Stream stream, Encoding? encoding = null) : base(stream, encoding ?? Encoding.UTF8)
    {
    }

    /// <summary>
    /// Reads a string from the stream. Assumes that the string is length-prefixed with an unsigned 16-bit integer.
    /// </summary>
    /// <returns>The string value.</returns>
    private string ReadString()
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        BaseStream.ReadExactly(buffer);
        var length = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        return ReadString(length);
    }

    /// <summary>
    /// Reads a signed 32-bit integer from the stream.
    /// </summary>
    /// <returns>The integer value.</returns>
    private int ReadInt32()
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BaseStream.ReadExactly(buffer);
        return BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }
    
    /// <inheritdoc />
    public override ByteTag ReadByte(bool named)
    {
        var name = named ? ReadString() : null;
        return new ByteTag(name, BaseStream.ReadByte());
    }

    /// <inheritdoc />
    public override ShortTag ReadShort(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        BaseStream.ReadExactly(buffer);
        return new ShortTag(name, BinaryPrimitives.ReadInt16LittleEndian(buffer));
    }

    /// <inheritdoc />
    public override IntTag ReadInt(bool named)
    {
        var name = named ? ReadString() : null;
        return new IntTag(name, ReadInt32());
    }

    /// <inheritdoc />
    public override LongTag ReadLong(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BaseStream.ReadExactly(buffer);
        return new LongTag(name, BinaryPrimitives.ReadInt32LittleEndian(buffer));
    }

    /// <inheritdoc />
    public override FloatTag ReadFloat(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BaseStream.ReadExactly(buffer);
        return new FloatTag(name, BinaryPrimitives.ReadSingleLittleEndian(buffer));
    }

    /// <inheritdoc />
    public override DoubleTag ReadDouble(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BaseStream.ReadExactly(buffer);
        return new DoubleTag(name, BinaryPrimitives.ReadDoubleLittleEndian(buffer));
    }

    /// <inheritdoc />
    public override StringTag ReadString(bool named)
    {
        var name = named ? ReadString() : null;
        return new StringTag(name, ReadString());
    }

    /// <inheritdoc />
    public override ByteArrayTag ReadByteArray(bool named)
    {
        var name = named ? ReadString() : null;
        var length = ReadInt32();
        if (length == 0)
            return new ByteArrayTag(name, Array.Empty<byte>());

        var values = new byte[length];
        BaseStream.ReadExactly(values, 0, length);
        return new ByteArrayTag(name, values);
    }

    /// <inheritdoc />
    public override IntArrayTag ReadIntArray(bool named)
    {
        var name = named ? ReadString() : null;
        var length = ReadInt32();
        if (length == 0)
            return new IntArrayTag(name, Array.Empty<int>());

        var values = new int[length];
        BaseStream.ReadExactly(MemoryMarshal.Cast<int, byte>(values));
        
        if (!BitConverter.IsLittleEndian)
        {
            for (var i = 0; i < length; i++)
                values[i] = values[i].SwapEndian();
        }

        return new IntArrayTag(name, values);
    }

    /// <inheritdoc />
    public override LongArrayTag ReadLongArray(bool named)
    {
        var name = named ? ReadString() : null;
        var length = ReadInt32();
        if (length == 0)
            return new LongArrayTag(name, Array.Empty<long>());

        var values = new long[length];
        BaseStream.ReadExactly(MemoryMarshal.Cast<long, byte>(values));
        
        if (!BitConverter.IsLittleEndian)
        {
            for (var i = 0; i < length; i++)
                values[i] = values[i].SwapEndian();
        }

        return new LongArrayTag(name, values);
    }

    /// <inheritdoc />
    public override ListTag ReadList(bool named)
    {
        var name = named ? ReadString() : null;
        var childType = (TagType)BaseStream.ReadByte();
        var length = ReadInt32();

        var list = new ListTag(name);
        for (var i = 0; i < length; i++)
            list.Add(ReadTag(false, childType));

        return list;
    }

    /// <inheritdoc />
    public override CompoundTag ReadCompound(bool named)
    {
        var name = named ? ReadString() : null;
        var compound = new CompoundTag(name);
        
        while (true)
        {
            var child = ReadTag(true);
            if (child is EndTag)
                break;
            compound.Add(child);
        }
        
        return compound;
    }
}