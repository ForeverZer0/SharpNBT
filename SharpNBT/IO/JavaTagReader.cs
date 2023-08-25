using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using SharpNBT.Tags;

namespace SharpNBT;

public class JavaTagReader : TagReader
{
    public JavaTagReader(Stream stream, Encoding? encoding = null) : base(stream, encoding ?? Encoding.UTF8)
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
        var length = BinaryPrimitives.ReadUInt16BigEndian(buffer);
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
        return BinaryPrimitives.ReadInt32BigEndian(buffer);
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
        return new ShortTag(name, BinaryPrimitives.ReadInt16BigEndian(buffer));
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
        return new LongTag(name, BinaryPrimitives.ReadInt32BigEndian(buffer));
    }

    /// <inheritdoc />
    public override FloatTag ReadFloat(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BaseStream.ReadExactly(buffer);
        return new FloatTag(name, BinaryPrimitives.ReadSingleBigEndian(buffer));
    }

    /// <inheritdoc />
    public override DoubleTag ReadDouble(bool named)
    {
        var name = named ? ReadString() : null;
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BaseStream.ReadExactly(buffer);
        return new DoubleTag(name, BinaryPrimitives.ReadDoubleBigEndian(buffer));
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
        
        if (BitConverter.IsLittleEndian)
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
        
        if (BitConverter.IsLittleEndian)
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