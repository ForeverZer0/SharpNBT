using System;
using System.Buffers;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT.IO;

/// <summary>
/// Abstract base class for types that can read NBT tags from a stream.
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
    public EventHandler<TagEventArgs>? TagRead;

    /// <summary>
    /// Occurs when the a tag is encountered in the stream, but before it has been processed by the reader.
    /// </summary>
    public EventHandler<TagHandledEventArgs>? TagEncountered;
    
    /// <summary>
    /// Gets the base stream that is the source for the reader.
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
    /// Reads a tag from the current position in the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of
    /// a <see cref="ListTag"/>.
    /// </param>
    /// <param name="type">
    /// The type of the tag to read, or <c>null</c> to indicate it is unknown, in which case it is assumed that it
    /// should be read as the next byte from the stream.
    /// </param>
    /// <returns>The deserialized tag.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// When an invalid tag type is supplied
    /// <para>-- or --</para>
    /// an invalid tag type is read from the stream, indicating it is positioned incorrectly.
    /// </exception>
    public Tag ReadTag(bool named, TagType? type = null)
    {
        type ??= (TagType)BaseStream.ReadByte();
        if (TagEncountered != null)
        {
            var args = new TagHandledEventArgs(named, type.Value);
            TagEncountered.Invoke(this, args);
            if (args is { Handled: true, Result: not null })
                return args.Result;
        }
        
        Tag value = type.Value switch
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
        
        TagRead?.Invoke(this, new TagEventArgs(value, type.Value));
        return value;
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
    public abstract ByteTag ReadByte(bool named);
    
    /// <summary>
    /// Reads a <see cref="ShortTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="ShortTag"/> instance.</returns>
    public abstract ShortTag ReadShort(bool named);
    
    /// <summary>
    /// Reads a <see cref="IntTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="IntTag"/> instance.</returns>
    public abstract IntTag ReadInt(bool named);
    
    /// <summary>
    /// Reads a <see cref="LongTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="LongTag"/> instance.</returns>
    public abstract LongTag ReadLong(bool named);
    
    /// <summary>
    /// Reads a <see cref="FloatTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="FloatTag"/> instance.</returns>
    public abstract FloatTag ReadFloat(bool named);
    
    /// <summary>
    /// Reads a <see cref="DoubleTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="DoubleTag"/> instance.</returns>
    public abstract DoubleTag ReadDouble(bool named);
    
    /// <summary>
    /// Reads a <see cref="StringTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="StringTag"/> instance.</returns>
    public abstract StringTag ReadString(bool named);
    
    /// <summary>
    /// Reads a <see cref="ByteArrayTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="ByteArrayTag"/> instance.</returns>
    public abstract ByteArrayTag ReadByteArray(bool named);
    
    /// <summary>
    /// Reads a <see cref="IntArrayTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="IntArrayTag"/> instance.</returns>
    public abstract IntArrayTag ReadIntArray(bool named);
    
    /// <summary>
    /// Reads a <see cref="LongArrayTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="LongArrayTag"/> instance.</returns>
    public abstract LongArrayTag ReadLongArray(bool named);
    
    /// <summary>
    /// Reads a <see cref="ListTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="ListTag"/> instance.</returns>
    public abstract ListTag ReadList(bool named);
    
    /// <summary>
    /// Reads a <see cref="CompoundTag"/> from the stream.
    /// </summary>
    /// <param name="named">
    /// Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a
    /// <see cref="ListTag"/>.
    /// </param>
    /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
    /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
    public abstract CompoundTag ReadCompound(bool named);
}