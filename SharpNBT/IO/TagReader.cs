using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT.IO;

/// <summary>
/// Handler for NBT deserialization as a tag is being read from a stream.
/// </summary>
/// <param name="reader">The <see cref="TagReader"/> instance invoking the event, which also provides access to the underlying stream.</param>
/// <param name="named">Flag indicating if the tag has a name that should be read from the stream.</param>
/// <param name="type">A constant describing the NBT tag type.</param>
/// <param name="tag">
/// The value of the tag when the handler has handled the deserialization, otherwise it can safely
/// be assigned a <see langword="null"/> or default value.</param>
/// <returns>
/// <see langword="true"/> when the read operation has been handled, in which case <paramref name="tag"/> has been
/// assigned a valid value, otherwise <see langword="false"/> if it has been not handled, and <paramref name="tag"/>
/// can safely be assign a <see langword="null"/> value.
/// </returns>
public delegate bool TagReadHandler(TagReader reader, bool named, TagType type, out Tag tag);

/// <summary>
/// Concrete implementation of a <see cref="TagReader"/> to provide shared functionality.
/// </summary>
[PublicAPI]
public abstract class TagReader
{
    /// <summary>
    /// Maximum number of bytes that will be allocated on the stack.
    /// </summary>
    protected const int StackAllocMax = 512;
    
    /// <summary>
    /// A memory pool that can be used for fast allocations of temporary memory.
    /// </summary>
    protected static ArrayPool<byte> MemoryPool => ArrayPool<byte>.Shared;
    
    /// <summary>
    /// Occurs when a tag has been deserialized from the stream.
    /// </summary>
    public event EventHandler<TagEventArgs>? TagRead;
    
    /// <summary>
    /// Gets the underlying stream.
    /// </summary>
    public Stream BaseStream { get; }
    
    /// <summary>
    /// Gets the encoding used for strings.
    /// </summary>
    public Encoding Encoding { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TagReader"/> class.
    /// </summary>
    /// <param name="stream">The base stream instance that is being read from.</param>
    /// <param name="encoding">The text-encoding to use for strings.</param>
    /// <exception cref="IOException">When the stream is invalid or not opened for reading.</exception>
    protected TagReader(Stream stream, Encoding encoding)
    {
        if (!stream.CanRead)
            throw new IOException("Stream is not opened for reading.");
        BaseStream = stream;
        Encoding = encoding;
    }
    
    /// <summary>
    /// Reads a tag of the given type from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of
    /// a <see cref="ListTag"/>.
    /// </param>
    /// <param name="type">The type of tag to read, or <see langword="null"/> to read the type from the stream.</param>
    /// <returns>The deserialized tag instance.</returns>
    public Tag Read(bool named, TagType? type = null)
    {
        type ??= (TagType)BaseStream.ReadByte();

        if (readHandler != null && readHandler.Invoke(this, named, type.Value, out var tag))
        {
            OnTagRead(tag, type.Value);
            return tag;
        }
        
        tag = type.Value switch
        {
            TagType.End => new EndTag(),
            TagType.Byte => ReadByte(named),
            TagType.Short => ReadShort(named),
            TagType.Int => ReadInt(named),
            TagType.Long => ReadLong(named),
            TagType.Float => ReadFloat(named),
            TagType.Double => ReadDouble(named),
            TagType.ByteArray => ReadByteArray(named),
            TagType.String => ReadString(named),
            TagType.List => ReadList(named),
            TagType.Compound => ReadCompound(named),
            TagType.IntArray => ReadIntArray(named),
            TagType.LongArray => ReadLongArray(named),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        OnTagRead(tag, type.Value);
        return tag;
    }

    /// <summary>
    /// Invokes the <see cref="TagRead"/> event when a tag is read from the stream.
    /// </summary>
    /// <param name="tag">The deserialized tag.</param>
    /// <param name="type">A constant describing the tag type.</param>
    protected virtual void OnTagRead(Tag tag, TagType type)
    {
        TagRead?.Invoke(this, new TagEventArgs(tag, type));
    }
    
    /// <summary>
    /// Sets a callback that will be triggered for each tag prior to it being read from the stream.
    /// </summary>
    /// <param name="handler">The handler to receive the callback, or <see langword="null"/> to unset it.</param>
    /// <returns>The callback that was previously set, or <see langword="null"/>.</returns>
    public TagReadHandler? SetHandler(TagReadHandler? handler)
    {
        var current = readHandler;
        readHandler = handler;
        return current;
    }
    
    /// <summary>
    /// Reads a tag of the given type from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of
    /// a <see cref="ListTag"/>.
    /// </param>
    /// <typeparam name="TTag">A tag type that implements <see cref="ITag"/>.</typeparam>
    /// <returns>The deserialized tag instance.</returns>
    /// <exception cref="InvalidOperationException">The tag is not of the type specified by <typeparamref name="TTag"/>.</exception>
    public TTag Read<TTag>(bool named = true) where TTag : Tag, ITag
    {
        var tag = Read(named);
        if (tag is TTag value)
            return value;
        
        throw new InvalidOperationException("The tag was not of the specified type.");
    }
    
    /// <summary>
    /// Reads a tag of the given type from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of
    /// a <see cref="ListTag"/>.
    /// </param>
    /// <param name="type">The type of tag to read, or <see langword="null"/> to read the type from the stream.</param>
    /// <param name="token">A synchronization object for providing task cancellation support.</param>
    /// <returns>The deserialized tag instance.</returns>
    public virtual Task<Tag> ReadAsync(bool named, TagType? type = null, CancellationToken token = default)
    {
        return Task.Run(() => Read(named, type), token);
    }

    /// <summary>
    /// Asynchronously reads a tag of the given type from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of
    /// a <see cref="ListTag"/>.
    /// </param>
    /// <param name="token">A synchronization object for providing task cancellation support.</param>
    /// <typeparam name="TTag">A tag type that implements <see cref="ITag"/>.</typeparam>
    /// <returns>The deserialized tag instance.</returns>
    /// <exception cref="InvalidOperationException">The tag is not of the type specified by <typeparamref name="TTag"/>.</exception>
    public virtual Task<TTag> ReadAsync<TTag>(bool named = true, CancellationToken token = default) where TTag : Tag, ITag
    {
        return Task.Run(() => Read<TTag>(named), token);
    }
    
    /// <summary>
    /// Read a <see cref="string"/> of the specified <paramref name="length"/> from the underlying stream, interpreted
    /// using the configured <see cref="Encoding"/>.
    /// </summary>
    /// <returns>The value read from the stream.</returns>
    protected string ReadString(int length)
    {
        if (length <= StackAllocMax)
        {
            Span<byte> buffer = stackalloc byte[length];
            BaseStream.ReadExactly(buffer);
            return Encoding.GetString(buffer);
        }

        var bytes = MemoryPool.Rent(length);
        try
        {
            BaseStream.ReadExactly(bytes, 0, length);
            return Encoding.GetString(bytes, 0, length);
        }
        finally
        {
            MemoryPool.Return(bytes);
        }
    }
    
    /// <summary>
    /// Reads a <see cref="ByteTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="ByteTag"/> instance.</returns>
    protected abstract ByteTag ReadByte(bool named);
    
    /// <summary>
    /// Reads a <see cref="ShortTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="ShortTag"/> instance.</returns>
    protected abstract ShortTag ReadShort(bool named);
    
    /// <summary>
    /// Reads a <see cref="IntTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="IntTag"/> instance.</returns>
    protected abstract IntTag ReadInt(bool named);
    
    /// <summary>
    /// Reads a <see cref="LongTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="LongTag"/> instance.</returns>
    protected abstract LongTag ReadLong(bool named);
    
    /// <summary>
    /// Reads a <see cref="FloatTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="FloatTag"/> instance.</returns>
    protected abstract FloatTag ReadFloat(bool named);
    
    /// <summary>
    /// Reads a <see cref="DoubleTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="DoubleTag"/> instance.</returns>
    protected abstract DoubleTag ReadDouble(bool named);
    
    /// <summary>
    /// Reads a <see cref="StringTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="StringTag"/> instance.</returns>
    protected abstract StringTag ReadString(bool named);
    
    /// <summary>
    /// Reads a <see cref="ByteArrayTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="ByteArrayTag"/> instance.</returns>
    protected abstract ByteArrayTag ReadByteArray(bool named);
    
    /// <summary>
    /// Reads a <see cref="IntArrayTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="IntArrayTag"/> instance.</returns>
    protected abstract IntArrayTag ReadIntArray(bool named);
    
    /// <summary>
    /// Reads a <see cref="LongArrayTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="LongArrayTag"/> instance.</returns>
    protected abstract LongArrayTag ReadLongArray(bool named);
    
    /// <summary>
    /// Reads a <see cref="ListTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="IListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="IListTag"/> instance.</returns>
    protected abstract IListTag ReadList(bool named);
    
    /// <summary>
    /// Reads a <see cref="CompoundTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
    protected abstract CompoundTag ReadCompound(bool named);

    private TagReadHandler? readHandler;
}