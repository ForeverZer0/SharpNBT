using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using SharpNBT.Tags;

namespace SharpNBT;

public class BedrockNetworkReader : TagReader
{
    public BedrockNetworkReader(Stream stream, Encoding? encoding = null) : base(stream, encoding ?? Encoding.UTF8)
    {
    }

    private ulong ReadVar(int bits)
    {
        var shift = 0;
        ulong result = 0;
        while (true)
        {
            var byteValue = (ulong) BaseStream.ReadByte();
            var tmp = byteValue & 0x7F;
            result |= tmp << shift;
            if (shift > bits)
                throw new OverflowException($"Variable-length integer cannot be contained in {bits} bits.");
            if ((byteValue & 0x80) != 0x80)
                return result;
                
            shift += 7;
        }
    }
    
    private int ReadVarInt()
    {
        // Bedrock uses zig-zag encoding on varints
        var value = ReadVar(32);
        unchecked
        {
            if ((value & 0x1) == 0x1)
            {
                return (int)(-1 * ((long)(value >> 1) + 1));
            }
            return (int)(value >> 1);
        }
    }

    private long ReadVarLong()
    {
        var value = ReadVar(64);
        unchecked
        {
            if ((value & 0x1) == 0x1)
            {
                return (-1 * ((long)(value >> 1) + 1));
            }
            return (long)(value >> 1);
        }
    }
    
    /// <summary>
    /// Reads a string from the stream. Assumes that the string is length-prefixed with a variable-length integer.
    /// </summary>
    /// <returns>The string value.</returns>
    private string ReadString()
    {
        var length = ReadVarInt();
        return ReadString(length);
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
        return new IntTag(name, ReadVarInt());
    }

    /// <inheritdoc />
    public override LongTag ReadLong(bool named)
    {
        var name = named ? ReadString() : null;
        return new LongTag(name, ReadVarLong());
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
        byte[] values;
        var name = named ? ReadString() : null;
        var length = ReadVarInt();
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
    public override IntArrayTag ReadIntArray(bool named)
    {
        var name = named ? ReadString() : null;
        var length = ReadVarInt();
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
        var length = ReadVarInt();
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
        var length = ReadVarInt();

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