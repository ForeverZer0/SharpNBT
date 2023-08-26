using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpNBT.IO;

public class JavaTagWriter : TagWriter
{
    public JavaTagWriter(Stream stream, Encoding encoding) : base(stream, encoding)
    {
    }

    private int WriteTypeAndName<TTag>(TTag tag, bool named, bool typed) where TTag : ITag
    {
        var count = 0;
        if (typed)
        {
            BaseStream.WriteByte((byte)TTag.Type);
            count++;
        }

        if (named)
            count += WriteStringValue(tag.Name);
        
        return count;
    }

    private int WriteInt32(int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        BaseStream.Write(buffer);
        return sizeof(int);
    }
    
    private int WriteStringValue(string? value)
    {
        var length = string.IsNullOrEmpty(value) ? 0 : Encoding.GetByteCount(value);
        if (length >= ushort.MaxValue)
            throw new FormatException($"String may not exceed {sizeof(ushort)} bytes in size.");

        Span<byte> sizeBuffer = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(sizeBuffer, unchecked((ushort)length));
        BaseStream.Write(sizeBuffer);
        
        var written = sizeof(ushort) + length;
        if (length == 0)
            return written;

        if (written <= StackAllocMax)
        {
            Span<byte> buffer = stackalloc byte[length];
            Encoding.GetBytes(value, buffer);
            BaseStream.Write(buffer);
            return written;
        }

        var bytes = MemoryPool.Rent(length);
        try
        {
            var span = new Span<byte>(bytes, 0, length);
            Encoding.GetBytes(value, span);
            BaseStream.Write(span);
            return written;
        }
        finally
        {
            MemoryPool.Return(bytes);
        }
    }
    
    /// <inheritdoc />
    protected override int WriteByte(ByteTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        BaseStream.WriteByte(tag.Value);
        return written + 1;
    }

    /// <inheritdoc />
    protected override int WriteShort(ShortTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(buffer, tag.Value);
        BaseStream.Write(buffer);
        return written + sizeof(short);
    }

    /// <inheritdoc />
    protected override int WriteInt(IntTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(buffer, tag.Value);
        BaseStream.Write(buffer);
        return written + sizeof(int);
    }

    /// <inheritdoc />
    protected override int WriteLong(LongTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(buffer, tag.Value);
        BaseStream.Write(buffer);
        return written + sizeof(long);
    }

    /// <inheritdoc />
    protected override int WriteFloat(FloatTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BinaryPrimitives.WriteSingleBigEndian(buffer, tag.Value);
        BaseStream.Write(buffer);
        return written + sizeof(float);
    }

    /// <inheritdoc />
    protected override int WriteDouble(DoubleTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BinaryPrimitives.WriteDoubleBigEndian(buffer, tag.Value);
        BaseStream.Write(buffer);
        return written + sizeof(double);
    }

    /// <inheritdoc />
    protected override int WriteString(StringTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        return written + WriteStringValue(tag.Value);
    }

    /// <inheritdoc />
    protected override int WriteByteArray(ByteArrayTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        written += WriteInt32(tag.Count);
        BaseStream.Write(tag, 0, tag.Count);
        return written + tag.Count;
    }

    /// <inheritdoc />
    protected override int WriteIntArray(IntArrayTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        written += WriteInt32(tag.Count);
        
        var byteSize = sizeof(int) * tag.Count;
        written += byteSize;
        if (!BitConverter.IsLittleEndian)
        {
            BaseStream.Write(MemoryMarshal.Cast<int, byte>(tag));
            return written;
        }
        
        if (byteSize <= StackAllocMax)
        {
            Span<int> buffer = stackalloc int[tag.Count];
            for (var i = 0; i < tag.Count; i++)
                buffer[i] = tag[i].SwapEndian();

            BaseStream.Write(MemoryMarshal.Cast<int, byte>(buffer));
            return written;
        }

        var bytes = MemoryPool.Rent(byteSize);
        try
        {
            var intBuffer = MemoryMarshal.Cast<byte, int>(new Span<byte>(bytes, 0, byteSize));
            for (var i = 0; i < tag.Count; i++)
                intBuffer[i] = tag[i].SwapEndian();
            BaseStream.Write(bytes, 0, byteSize);
            return written;
        }
        finally
        {
            MemoryPool.Return(bytes);
        }
    }

    /// <inheritdoc />
    protected override int WriteLongArray(LongArrayTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        written += WriteInt32(tag.Count);
        
        var byteSize = sizeof(long) * tag.Count;
        written += byteSize;
        if (!BitConverter.IsLittleEndian)
        {
            BaseStream.Write(MemoryMarshal.Cast<long, byte>(tag));
            return written;
        }
        
        if (byteSize <= StackAllocMax)
        {
            Span<long> buffer = stackalloc long[tag.Count];
            for (var i = 0; i < tag.Count; i++)
                buffer[i] = tag[i].SwapEndian();

            BaseStream.Write(MemoryMarshal.Cast<long, byte>(buffer));
            return written;
        }

        var bytes = MemoryPool.Rent(byteSize);
        try
        {
            var intBuffer = MemoryMarshal.Cast<byte, long>(new Span<byte>(bytes, 0, byteSize));
            for (var i = 0; i < tag.Count; i++)
                intBuffer[i] = tag[i].SwapEndian();
            BaseStream.Write(bytes, 0, byteSize);
            return written;
        }
        finally
        {
            MemoryPool.Return(bytes);
        }
    }
    
    /// <inheritdoc />
    protected override int WriteList(IListTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        
        BaseStream.WriteByte((byte)tag.ChildType);
        written += 1 + WriteInt32(tag.Count);
        
        foreach (var child in tag)
        {
            written += Write(child, false, false);
        }
        
        return written;
    }

    /// <inheritdoc />
    protected override int WriteCompound(CompoundTag tag, bool named, bool typed)
    {
        var written = WriteTypeAndName(tag, named, typed);
        foreach (var kvp in tag)
        {
            // ReSharper disable RedundantArgumentDefaultValue
            written += Write(kvp.Value, true, true);
            // ReSharper restore RedundantArgumentDefaultValue
        }
        return written;
    }
}