using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Delegate type for tag-related events that can occur within the <see cref="TagReader"/> class.
    /// </summary>
    /// <seealso cref="TagReader.TagRead"/>
    public delegate void TagReadCallback(TagReader reader, TagType type, Tag tag);
    

    /// <summary>
    /// Provides methods for reading NBT data from a stream.
    /// </summary>
    [PublicAPI]
    public class TagReader : IDisposable
    {
        /// <summary>
        /// Occurs when a tag has been fully deserialized from the stream.
        /// </summary>
        public event TagReadCallback TagRead;

        /// <summary>
        /// Gets the underlying stream this <see cref="TagReader"/> is operating on.
        /// </summary>
        [NotNull]
        protected Stream BaseStream { get; }
        
        private readonly bool leaveOpen;

        /// <summary>
        /// Creates a new instance of the <see cref="TagReader"/> class from the given uncompressed <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance that the reader will be reading from.</param>
        /// <param name="leaveOpen">
        /// <paramref langword="true"/> to leave the <paramref name="stream"/> object open after disposing the <see cref="TagReader"/>
        /// object; otherwise, <see langword="false"/>.</param>
        public TagReader([NotNull] Stream stream, bool leaveOpen = false)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new IOException("Stream is not opened for reading.");
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Reads a <see cref="ByteTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ByteTag"/> instance.</returns>
        public ByteTag ReadByte(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            return new ByteTag(name, (byte) BaseStream.ReadByte());
        }

        /// <summary>
        /// Reads a <see cref="ShortTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ShortTag"/> instance.</returns>
        public ShortTag ReadShort(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            Span<byte> buffer = stackalloc byte[sizeof(short)];
            BaseStream.Read(buffer);
            var value = BitConverter.ToInt16(buffer);

            return new ShortTag(name, BitConverter.IsLittleEndian ? value.SwapEndian() : value);
        }
        
        /// <summary>
        /// Reads a <see cref="IntTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="IntTag"/> instance.</returns>
        public IntTag ReadInt(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            return new IntTag(name, ReadInt32());
        }
        
        /// <summary>
        /// Reads a <see cref="LongTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="LongTag"/> instance.</returns>
        public LongTag ReadLong(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BaseStream.Read(buffer);
            var value = BitConverter.ToInt64(buffer);

            return new LongTag(name, BitConverter.IsLittleEndian ? value.SwapEndian() : value);
        }
        
        /// <summary>
        /// Reads a <see cref="FloatTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="FloatTag"/> instance.</returns>
        public FloatTag ReadFloat(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            
            var buffer = new byte[sizeof(float)];
            BaseStream.Read(buffer, 0, sizeof(float));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            
            return new FloatTag( name, BitConverter.ToSingle(buffer));
        }

        /// <summary>
        /// Reads a <see cref="DoubleTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="DoubleTag"/> instance.</returns>
        public DoubleTag ReadDouble(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            var buffer = new byte[sizeof(double)];
            BaseStream.Read(buffer, 0, buffer.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            return new DoubleTag( name, BitConverter.ToDouble(buffer, 0));
        }
        
        /// <summary>
        /// Reads a <see cref="StringTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="StringTag"/> instance.</returns>
        public StringTag ReadString(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            var value = ReadUTF8String();
            return new StringTag(name, value);
        }
        
        /// <summary>
        /// Reads a <see cref="ByteArrayTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ByteArrayTag"/> instance.</returns>
        public ByteArrayTag ReadByteArray(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            var count = ReadInt32();
            var buffer = new byte[count];
            BaseStream.Read(buffer, 0, count);
            return new ByteArrayTag(name, buffer);
        }
        
        /// <summary>
        /// Reads a <see cref="IntArrayTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="IntArrayTag"/> instance.</returns>
        public IntArrayTag ReadIntArray(bool named = true)
        {
            const int INT_SIZE = sizeof(int);
            
            var name = named ? ReadUTF8String() : null;
            var count = ReadInt32();
            var buffer = new byte[count * INT_SIZE];
            BaseStream.Read(buffer, 0, count * INT_SIZE);

            Span<int> values = MemoryMarshal.Cast<byte, int>(buffer);
            if (BitConverter.IsLittleEndian)
            {
                for (var i = 0; i < count; i++)
                    values[i] = values[i].SwapEndian();
            }
            return new IntArrayTag(name, values);
        }
        
        /// <summary>
        /// Reads a <see cref="LongArrayTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="LongArrayTag"/> instance.</returns>
        public LongArrayTag ReadLongArray(bool named = true)
        {
            const int LONG_SIZE = sizeof(long);
            
            var name = named ? ReadUTF8String() : null;
            var count = ReadInt32();
            var buffer = new byte[count * LONG_SIZE];
            BaseStream.Read(buffer, 0, count * LONG_SIZE);

            Span<long> values = MemoryMarshal.Cast<byte, long>(buffer);
            if (BitConverter.IsLittleEndian)
            {
                for (var i = 0; i < count; i++)
                    values[i] = values[i].SwapEndian();
            }
            return new LongArrayTag(name, values);
        }
        
        /// <summary>
        /// Reads a <see cref="ListTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ListTag"/> instance.</returns>
        public ListTag ReadList(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            var childType = ReadType();
            var count = ReadInt32();
            
            if (childType == TagType.End && count > 0)
                throw new FormatException("An EndTag is not a valid child type for a non-empty ListTag.");
            
            var list = new ListTag(name, childType);
            while (count-- > 0)
            {
                list.Add(ReadTag(childType, false));
            }
            return list;
        }
        
        
        /// <summary>
        /// Reads a <see cref="CompoundTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
        public CompoundTag ReadCompound(bool named = true)
        {
            var name = named ? ReadUTF8String() : null;
            var compound = new CompoundTag(name);
            
            while (true)
            {
                var type = ReadType();
                if (type == TagType.End)
                    break;
                
                compound.Add(ReadTag(type, true));
            }

            return compound;
        }
        
        /// <summary>
        /// Reads a <see cref="Tag"/> from the current position in the stream. 
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <returns>The tag instance that was read from the stream.</returns>
        public Tag ReadTag(bool named = true)
        {
            var type = ReadType();
            return ReadTag(type, named);
        }

        /// <summary>
        /// Asynchronously reads a <see cref="Tag"/> from the current position in the stream. 
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <returns>The tag instance that was read from the stream.</returns>
        public async Task<Tag> ReadTagAsync(bool named = true)
        {
            return await Task.Run(() => ReadTag(named));
        }

        /// <summary>
        /// Convenience method to read a tag and cast it automatically.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <typeparam name="T">The tag type that is being read from the stream.</typeparam>
        /// <returns>The tag instance that was read from the stream.</returns>
        /// <remarks>This is typically only used when reading the top-level <see cref="CompoundTag"/> of a document where the type is already known.</remarks>
        public T ReadTag<T>(bool named = true) where T : Tag
        {
            return (T)ReadTag(named);
        }

        /// <summary>
        /// Convenience method to asynchronously read a tag and cast it automatically.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <typeparam name="T">The tag type that is being read from the stream.</typeparam>
        /// <returns>The tag instance that was read from the stream.</returns>
        /// <remarks>This is typically only used when reading the top-level <see cref="CompoundTag"/> of a document where the type is already known.</remarks>
        public async Task<T> ReadTagAsync<T>(bool named = true) where T : Tag
        {
            var tag = await ReadTagAsync(named);
            return (T)tag;
        }
        
        [NotNull] 
        private Tag ReadTag(TagType type, bool named)
        {
            Tag tag = type switch
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
            OnTagRead(tag);
            return tag;
        }
        
        
        private TagType ReadType()
        {
            try
            {
                return (TagType)BaseStream.ReadByte();
            }
            catch (EndOfStreamException)
            {
                return TagType.End;
            }
        }
        
        
        
        

        
        
        
        
        
        
        
        /// <summary>
        /// Reads a length-prefixed UTF-8 string from the stream.
        /// </summary>
        /// <returns>The deserialized string instance.</returns>
        [CanBeNull]
        protected string ReadUTF8String()
        {
            Span<byte> buffer = stackalloc byte[sizeof(ushort)];
            BaseStream.Read(buffer);
            var length = BitConverter.ToUInt16(buffer);
            if (BitConverter.IsLittleEndian)
                length = length.SwapEndian();

            if (length == 0)
                return null;
            
            var utf8 = new byte[length];
            BaseStream.Read(utf8, 0, length);
            return Encoding.UTF8.GetString(utf8);
        }

        /// <summary>
        /// Reads a 64-bit signed (big-endian) integer from the stream, converting to native endian when necessary.
        /// </summary>
        /// <returns>The deserialized value.</returns>
        private int ReadInt32()
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            BaseStream.Read(buffer);
            var value = BitConverter.ToInt32(buffer);
            return BitConverter.IsLittleEndian ? value.SwapEndian() : value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!leaveOpen)
                BaseStream.Dispose();
        }

        /// <summary>
        /// Asynchronously releases the unmanaged resources used by the <see cref="TagReader"/>.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!leaveOpen)
                await BaseStream.DisposeAsync();
        }

        /// <summary>
        /// Invokes the <see cref="TagRead"/> event when a tag has been fully deserialized from the <see cref="BaseStream"/>.
        /// </summary>
        /// <param name="tag">The deserialized <see cref="Tag"/> instance.</param>
        protected virtual void OnTagRead(Tag tag) =>  TagRead?.Invoke(this, tag.Type, tag);
        
    }
}