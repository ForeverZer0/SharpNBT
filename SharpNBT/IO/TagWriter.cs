using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpNBT.IO;

/// <summary>
/// Handler for NBT serialization as a tag is being written to a stream.
/// </summary>
/// <param name="writer">The <see cref="TagWriter"/> instance invoking the event, which also provides access to the underlying stream..</param>
/// <param name="tag">The <see cref="Tag"/> instance that is triggering the event.</param>
/// <param name="type">A constant describing the NBT tag type.</param>
/// <param name="named"><see langword="true"/> if tag's name should be written, otherwise <see langword="false"/>.</param>
/// <param name="bytesWritten">
/// When handled, this must be the number of bytes written to the stream, otherwise <c>0</c>.
/// </param>
/// <returns>
/// <see langword="true"/> when the read operation has been handled, otherwise <see langword="false"/>.
/// </returns>
/// <remarks>
/// This is useful for providing special handling of a particular tag/type, as well as simply monitoring tag writes.
/// </remarks>
public delegate bool TagWriteHandler(TagWriter writer, Tag tag, TagType type, bool named, out int bytesWritten);

/// <summary>
/// Concrete implementation of a <see cref="TagWriter"/> to provide shared functionality.
/// </summary>
public abstract class TagWriter
{
    /// <summary>
    /// Gets the underlying stream.
    /// </summary>
    Stream BaseStream { get; }
    
    /// <summary>
    /// Gets the encoding used for strings.
    /// </summary>
    Encoding Encoding { get; }
    
    /// <summary>
    /// Occurs when a tag has been serialized to the stream.
    /// </summary>
    public event EventHandler<TagEventArgs>? TagWritten;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TagWriter"/> class.
    /// </summary>
    /// <param name="stream">The base stream instance that is being written to.</param>
    /// <param name="encoding">The text-encoding to use for strings.</param>
    /// <exception cref="IOException">When the stream is invalid or not opened for writing.</exception>
    protected TagWriter(Stream stream, Encoding encoding)
    {
        if (!stream.CanWrite)
            throw new IOException("Stream is not opened for writing.");
        BaseStream = stream;
        Encoding = encoding;
    }
    
    /// <summary>
    /// Serializes an NBT tag to the underlying stream.
    /// </summary>
    /// <param name="tag">The NBT tag to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <typeparam name="TTag">A <see cref="Tag"/> type that implements <see cref="ITag"/>.</typeparam>
    /// <returns>The number of bytes written to the stream.</returns>
    public int Write<TTag>(TTag tag, bool named = true) where TTag : Tag, ITag
    {
        if (writeHandler != null && writeHandler.Invoke(this, tag, TTag.Type, named, out var count))
        {
            OnTagWrite(tag, TTag.Type);
            return count;
        }
        
        BaseStream.WriteByte((byte)TTag.Type);
        count = 1 + tag switch
        {
            EndTag _ => WriteEnd(),
            ByteTag b => WriteByte(b, named),
            ShortTag s => WriteShort(s, named),
            IntTag i => WriteInt(i, named),
            LongTag l => WriteLong(l, named),
            FloatTag f => WriteFloat(f, named),
            DoubleTag d => WriteDouble(d, named),
            StringTag str => WriteString(str, named),
            ByteArrayTag ba => WriteByteArray(ba, named),
            IntArrayTag ia => WriteIntArray(ia, named),
            LongArrayTag la => WriteLongArray(la, named),
            ListTag list => WriteList(list, named),
            CompoundTag compound => WriteCompound(compound, named),
            _ => throw new ArgumentException("Unknown tag type.", nameof(tag))
        };
        
        OnTagWrite(tag, TTag.Type);
        return count;
    }
    
    /// <summary>
    /// Invokes the <see cref="TagWritten"/> event when a tag is written to the stream.
    /// </summary>
    /// <param name="tag">The serialized tag.</param>
    /// <param name="type">A constant describing the tag type.</param>
    protected virtual void OnTagWrite(Tag tag, TagType type)
    {
        TagWritten?.Invoke(this, new TagEventArgs(tag, type));
    }

    /// <summary>
    /// Asynchronously serializes an NBT tag to the underlying stream.
    /// </summary>
    /// <param name="tag">The NBT tag to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <param name="token">A synchronization object for providing task cancellation support.</param>
    /// <typeparam name="TTag">A <see cref="Tag"/> type that implements <see cref="ITag"/>.</typeparam>
    /// <returns>The number of bytes written to the stream.</returns>
    public virtual Task<int> WriteAsync<TTag>(TTag tag, bool named = true, CancellationToken token = default) where TTag : Tag, ITag
    {
        return Task.Run(() => Write(tag, named), token);
    }
    
    /// <summary>
    /// Sets a callback that will be triggered for each tag prior to it being written to the stream.
    /// </summary>
    /// <param name="handler">The handler to receive the callback, or <see langword="null"/> to unset it.</param>
    /// <returns>The callback that was previously set, or <see langword="null"/>.</returns>
    public TagWriteHandler? SetHandler(TagWriteHandler? handler)
    {
        var current = writeHandler;
        writeHandler = handler;
        return current;
    }
    
    protected int WriteEnd()
    {
        BaseStream.WriteByte(0);
        return 1;
    }
    
    /// <summary>
    /// Serializes a <see cref="ByteTag"/> to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteByte(ByteTag tag, bool named);
    
    /// <summary>
    /// Serializes a <see cref="ShortTag"/> to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteShort(ShortTag tag, bool named);
    
    /// <summary>
    /// Serializes a <see cref="IntTag"/> to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteInt(IntTag tag, bool named);
    
    /// <summary>
    /// Serializes a <see cref="LongTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteLong(LongTag tag, bool named);
    
    /// <summary>
    /// Serializes a <see cref="FloatTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteFloat(FloatTag tag, bool named);
    
    /// <summary>
    /// Serializes a <see cref="DoubleTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteDouble(DoubleTag tag, bool named);
    
    /// <summary>
    /// Serializes a <see cref="StringTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteString(StringTag tag, bool named);
    
    /// <summary>
    /// Serializes a <see cref="ByteArrayTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteByteArray(ByteArrayTag tag, bool named);

    /// <summary>
    /// Serializes a <see cref="IntArrayTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteIntArray(IntArrayTag tag, bool named);

    /// <summary>
    /// Serializes a <see cref="LongArrayTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteLongArray(LongArrayTag tag, bool named);

    /// <summary>
    /// Serializes a <see cref="ListTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteList(ListTag tag, bool named);

    /// <summary>
    /// Serializes a <see cref="CompoundTag"/> payload to the underlying stream.
    /// </summary>
    /// <param name="tag">The tag instance to write.</param>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>The number of bytes written.</returns>
    /// <remarks>Implementors must <b>not</b> write the type prefix.</remarks>
    protected abstract int WriteCompound(CompoundTag tag, bool named);

    private TagWriteHandler? writeHandler;
}