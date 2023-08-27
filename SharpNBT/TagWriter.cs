using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Provides methods for writing NBT tags to a stream.
/// </summary>
[PublicAPI]
public class TagWriter : TagIO
{
    private readonly bool leaveOpen;

    /// <summary>
    /// Creates a new instance of the <see cref="TagWriter"/> class from the given <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> instance that the writer will be writing to.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <param name="leaveOpen">
    /// <paramref langword="true"/> to leave the <paramref name="stream"/> object open after disposing the <see cref="TagWriter"/>
    /// object; otherwise, <see langword="false"/>.</param>
    public TagWriter(Stream stream, FormatOptions options, bool leaveOpen = false) : base(stream, options)
    {
        if (!stream.CanWrite)
            throw new IOException(Strings.CannotWriteStream);
        this.leaveOpen = leaveOpen;
    }

    /// <summary>
    /// Writes a <see cref="ByteTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="ByteTag"/> instance to write.</param>
    public virtual void WriteByte(ByteTag tag)
    {
        WriteTypeAndName(tag);
        BaseStream.WriteByte(tag.Value);
    }

    /// <summary>
    /// Writes a <see cref="ShortTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="ShortTag"/> instance to write.</param>
    public virtual void WriteShort(ShortTag tag)
    {
        WriteTypeAndName(tag);
        if (UseVarInt)
            VarInt.Write(BaseStream, tag.Value, ZigZagEncoding);
        else
            BaseStream.Write(GetBytes(tag.Value), 0, sizeof(short));
    }
        
    /// <summary>
    /// Writes a <see cref="IntTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="IntTag"/> instance to write.</param>
    public virtual void WriteInt(IntTag tag)
    {
        WriteTypeAndName(tag);
        if (UseVarInt)
            VarInt.Write(BaseStream, tag.Value, ZigZagEncoding);
        else
            BaseStream.Write(GetBytes(tag.Value), 0, sizeof(int));
    }

    /// <summary>
    /// Writes a <see cref="LongTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="LongTag"/> instance to write.</param>
    public virtual void WriteLong(LongTag tag)
    {
        WriteTypeAndName(tag);
        if (UseVarInt)
            VarLong.Write(BaseStream, tag.Value, ZigZagEncoding);
        else
            BaseStream.Write(GetBytes(tag.Value), 0, sizeof(long));
    }
        
    /// <summary>
    /// Writes a <see cref="FloatTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="FloatTag"/> instance to write.</param>
    public virtual void WriteFloat(FloatTag tag)
    {
        WriteTypeAndName(tag);
        BaseStream.Write(GetBytes(tag.Value), 0, sizeof(float));
    }

    /// <summary>
    /// Writes a <see cref="DoubleTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="DoubleTag"/> instance to write.</param>
    public virtual void WriteDouble(DoubleTag tag)
    {
        WriteTypeAndName(tag);
        BaseStream.Write(GetBytes(tag.Value), 0, sizeof(double));
    }
        
    /// <summary>
    /// Writes a <see cref="StringTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="StringTag"/> instance to write.</param>
    public virtual void WriteString(StringTag tag)
    {
        WriteTypeAndName(tag);
        WriteUTF8String(tag.Value);
    }
        
    /// <summary>
    /// Writes a <see cref="ByteArrayTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="ByteArrayTag"/> instance to write.</param>
    public virtual void WriteByteArray(ByteArrayTag tag)
    {
        WriteTypeAndName(tag);
        WriteCount(tag.Count);
        BaseStream.Write(tag.ToArray(), 0, tag.Count);
    }
        
    /// <summary>
    /// Writes a <see cref="IntArrayTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="IntArrayTag"/> instance to write.</param>
    public virtual void WriteIntArray(IntArrayTag tag)
    {
        WriteTypeAndName(tag);
        WriteCount(tag.Count);
            
        var values = new Span<int>(tag.ToArray());
        if (UseVarInt)
        {
            // VarInt is effectively always little-endian
            foreach (var n in values)
                VarInt.Write(BaseStream, n, ZigZagEncoding);
            return;
        }
        if (SwapEndian)
        {
            for (var i = 0; i < values.Length; i++)
                values[i] = values[i].SwapEndian();
        }
        BaseStream.Write(MemoryMarshal.AsBytes(values));
    }

    /// <summary>
    /// Writes a <see cref="LongArrayTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="LongArrayTag"/> instance to write.</param>
    public virtual void WriteLongArray(LongArrayTag tag)
    {

        WriteTypeAndName(tag);
        WriteCount(tag.Count);

        var values = new Span<long>(tag.ToArray());
        if (UseVarInt)
        {
            // VarLong is effectively always little-endian
            foreach (var n in values)
                VarLong.Write(BaseStream, n, ZigZagEncoding);
            return;
        }
        if (SwapEndian)
        {
            for (var i = 0; i < values.Length; i++)
                values[i] = values[i].SwapEndian();
                
        }
        BaseStream.Write(MemoryMarshal.AsBytes(values));
    }
        
    /// <summary>
    /// Writes a <see cref="ListTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="ListTag"/> instance to write.</param>
    public virtual void WriteList(ListTag tag)
    {
        WriteTypeAndName(tag);
        BaseStream.WriteByte((byte) tag.ChildType);
        WriteCount(tag.Count);
            
        foreach (var child in tag)
            WriteTag(child);
    }
        
        
    /// <summary>
    /// Writes a <see cref="CompoundTag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="CompoundTag"/> instance to write.</param>
    public virtual void WriteCompound(CompoundTag tag)
    {
        WriteTypeAndName(tag);
        foreach (var child in tag)
        {
            if (tag.Type == TagType.End)
                break;
                
            child.Parent = tag;
            WriteTag(child);
        }
            
        BaseStream.WriteByte((byte) TagType.End);
    }

    /// <summary>
    /// Convenience method to build and write a <see cref="TagBuilder"/> instance to the underlying stream.
    /// </summary>
    /// <param name="builder">A <see cref="TagBuilder"/> instance to write.</param>
    public virtual void WriteBuilder(TagBuilder builder) => WriteCompound(builder.Create());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    public virtual void WriteEndTag(EndTag? tag = null) => BaseStream.WriteByte(0); 
        
    /// <summary>
    /// Writes the given <see cref="Tag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="Tag"/> instance to be written.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the tag type is unrecognized.</exception>
    public void WriteTag(Tag tag)
    {
        switch (tag.Type)
        {
            case TagType.End: 
                WriteEndTag((EndTag) tag);
                break;
            case TagType.Byte:
                WriteByte((ByteTag) tag);
                break;
            case TagType.Short:
                WriteShort((ShortTag) tag);
                break;
            case TagType.Int:
                WriteInt((IntTag) tag);
                break;
            case TagType.Long:
                WriteLong((LongTag)tag);
                break;
            case TagType.Float:
                WriteFloat((FloatTag)tag);
                break;
            case TagType.Double:
                WriteDouble((DoubleTag)tag);
                break;
            case TagType.ByteArray:
                WriteByteArray((ByteArrayTag)tag);
                break;
            case TagType.String:
                WriteString((StringTag)tag);
                break;
            case TagType.List:
                WriteList((ListTag)tag);
                break;
            case TagType.Compound:
                WriteCompound((CompoundTag)tag);
                break;
            case TagType.IntArray:
                WriteIntArray((IntArrayTag)tag);
                break;
            case TagType.LongArray:
                WriteLongArray((LongArrayTag)tag);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tag.Type), Strings.UnknownTagType);
        }
    }

    /// <summary>
    /// Asynchronously writes the given <see cref="Tag"/> to the stream.
    /// </summary>
    /// <param name="tag">The <see cref="Tag"/> instance to be written.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the tag type is unrecognized.</exception>
    public async Task WriteTagAsync(Tag tag)
    {
        await Task.Run(() => WriteTag(tag));
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        BaseStream.Flush();
        if (!leaveOpen)
            BaseStream.Dispose();
    }

    /// <summary>
    /// Asynchronously releases the unmanaged resources used by the <see cref="TagReader"/>.
    /// </summary>
    public override async ValueTask DisposeAsync()
    {
        await BaseStream.FlushAsync();
        if (!leaveOpen)
            await BaseStream.DisposeAsync();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteTypeAndName(Tag tag)
    {
        if (tag.Parent is ListTag)
            return;

        BaseStream.WriteByte((byte) tag.Type);
        WriteUTF8String(tag.Name);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteUTF8String(string value)
    {
        // String length prefixes never use ZigZag encoding
            
        if (string.IsNullOrEmpty(value))
        {
            if (UseVarInt)
                VarInt.Write(BaseStream, 0);
            else
                BaseStream.Write(GetBytes((ushort) 0), 0, sizeof(ushort));
        }
        else
        {
            var utf8 = Encoding.UTF8.GetBytes(value);
            if (UseVarInt)
                VarInt.Write(BaseStream, utf8.Length);
            else
                BaseStream.Write(GetBytes((ushort) utf8.Length), 0, sizeof(ushort));
                
            BaseStream.Write(utf8, 0, utf8.Length);
        }
    }

    /// <summary>
    /// Gets the bytes for this number, accounting for the host machine endianness and target format.
    /// </summary>
    /// <param name="n">The value to convert.</param>
    /// <returns>An array of bytes representing the value in compatible format.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetBytes(short n) => BitConverter.GetBytes(SwapEndian ? n.SwapEndian() : n);

    /// <inheritdoc cref="GetBytes(short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetBytes(int n) => BitConverter.GetBytes(SwapEndian ? n.SwapEndian() : n);
        
    /// <inheritdoc cref="GetBytes(short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetBytes(long n) => BitConverter.GetBytes(SwapEndian ? n.SwapEndian() : n);
        
    /// <inheritdoc cref="GetBytes(short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetBytes(ushort n) => BitConverter.GetBytes(SwapEndian ? n.SwapEndian() : n);
        
    /// <inheritdoc cref="GetBytes(short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetBytes(float n)
    {
        var bytes = BitConverter.GetBytes(n);
        if (SwapEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    /// <inheritdoc cref="GetBytes(short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetBytes(double n)
    {
        var bytes = BitConverter.GetBytes(n);
        if (SwapEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    private void WriteCount(int count)
    {
        if (UseVarInt)
            VarInt.Write(BaseStream, count, ZigZagEncoding);
        else
            BaseStream.Write(GetBytes(count), 0, sizeof(int));
    }
}