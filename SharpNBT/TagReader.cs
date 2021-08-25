using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Provides methods for reading NBT data from a stream.
    /// </summary>
    [PublicAPI]
    public class TagReader : TagIO
    {
        /// <summary>
        /// Occurs when a tag has been fully deserialized from the stream.
        /// </summary>
        public event TagReaderCallback<TagEventArgs> TagRead;

        /// <summary>
        /// Occurs when a tag has been encountered in the stream, after reading the first byte to determine its <see cref="TagType"/>.
        /// </summary>
        public event TagReaderCallback<TagHandledEventArgs> TagEncountered;

        private readonly bool leaveOpen;

        /// <summary>
        /// Creates a new instance of the <see cref="TagReader"/> class from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance that the reader will be reading from.</param>
        /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
        /// <param name="leaveOpen">
        /// <paramref langword="true"/> to leave the <paramref name="stream"/> object open after disposing the <see cref="TagReader"/>
        /// object; otherwise, <see langword="false"/>.</param>
        public TagReader([NotNull] Stream stream, FormatOptions options, bool leaveOpen = false) : base(stream, options)
        {
            if (!stream.CanRead)
                throw new IOException(Strings.CannotReadStream);
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
            short value;

            if (UseVarInt)
            {
                value = (short)VarInt.Read(BaseStream, ZigZagEncoding);
            }
            else
            {
                Span<byte> buffer = stackalloc byte[sizeof(short)];
                BaseStream.Read(buffer);
                value = BitConverter.ToInt16(buffer);
                if (SwapEndian)
                    value = value.SwapEndian();
            }
            
            return new ShortTag(name, value);
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
            return new IntTag(name, UseVarInt ? VarInt.Read(BaseStream, ZigZagEncoding) : ReadInt32());
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
            long value;
            
            if (UseVarInt)
            {
                value = VarLong.Read(BaseStream, ZigZagEncoding);
            }
            else
            {
                Span<byte> buffer = stackalloc byte[sizeof(long)];
                BaseStream.Read(buffer);
                value = BitConverter.ToInt64(buffer);
                if (SwapEndian)
                    value = value.SwapEndian();
            }
            
            return new LongTag(name, value);
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
            if (SwapEndian)
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
            if (SwapEndian)
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
            var count = ReadCount();
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
            var count = ReadCount();
            
            if (UseVarInt)
            {
                var array = new int[count];
                for (var i = 0; i < count; i++)
                    array[i] = VarInt.Read(BaseStream, ZigZagEncoding);
                return new IntArrayTag(name, array);
            }
 
            var buffer = new byte[count * INT_SIZE];
            BaseStream.Read(buffer, 0, count * INT_SIZE);

            Span<int> values = MemoryMarshal.Cast<byte, int>(buffer);
            if (SwapEndian)
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
            var count = ReadCount();
            
            if (UseVarInt)
            {
                var array = new long[count];
                for (var i = 0; i < count; i++)
                    array[i] = VarLong.Read(BaseStream, ZigZagEncoding);
                return new LongArrayTag(name, array);
            }

            var buffer = new byte[count * LONG_SIZE];
            BaseStream.Read(buffer, 0, count * LONG_SIZE);

            Span<long> values = MemoryMarshal.Cast<byte, long>(buffer);
            if (SwapEndian)
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
            var count = ReadCount();
            
            if (childType == TagType.End && count > 0)
                throw new FormatException(Strings.InvalidEndTagChild);
            
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
            var result = OnTagEncountered(type, named);
            if (result != null)
            {
                OnTagRead(result);
                return result;
            }

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
            int length;
            if (UseVarInt)
                length = VarInt.Read(BaseStream);
            else
            {
                Span<byte> buffer = stackalloc byte[sizeof(ushort)];
                BaseStream.Read(buffer);
                var uint16 = BitConverter.ToUInt16(buffer);
                length = SwapEndian ? uint16.SwapEndian() : uint16;
            }

            if (length == 0)
                return null;
            
            var utf8 = new byte[length];
            BaseStream.Read(utf8, 0, length);
            return Encoding.UTF8.GetString(utf8);
        }

        private int ReadCount() => UseVarInt ? VarInt.Read(BaseStream, ZigZagEncoding) : ReadInt32();

        /// <summary>
        /// Reads a 64-bit signed (big-endian) integer from the stream, converting to native endian when necessary.
        /// </summary>
        /// <returns>The deserialized value.</returns>
        private int ReadInt32()
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            BaseStream.Read(buffer);
            var value = BitConverter.ToInt32(buffer);
            return SwapEndian ? value.SwapEndian() : value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (!leaveOpen)
                BaseStream.Dispose();
        }

        /// <summary>
        /// Asynchronously releases the unmanaged resources used by the <see cref="TagReader"/>.
        /// </summary>
        public override async ValueTask DisposeAsync()
        {
            if (!leaveOpen)
                await BaseStream.DisposeAsync();
        }

        /// <summary>
        /// Invokes the <see cref="TagRead"/> event when a tag has been fully deserialized from the <see cref="TagIO.BaseStream"/>.
        /// </summary>
        /// <param name="tag">The deserialized <see cref="Tag"/> instance.</param>
        protected virtual void OnTagRead(Tag tag) =>  TagRead?.Invoke(this, new TagEventArgs(tag.Type, tag));

        /// <summary>
        /// Invokes the <see cref="TagEncountered"/> event when the stream is positioned at the beginning of a an unread tag.
        /// </summary>
        /// <param name="type">The type of tag next to be read from the stream.</param>
        /// <param name="named">Flag indicating if this tag is named.</param>
        /// <returns>When handled by an event subscriber, returns a parsed <see cref="Tag"/> instance, otherwise returns <param langword="null">.</param></returns>
        [CanBeNull]
        protected virtual Tag OnTagEncountered(TagType type, bool named)
        {
            // Early out if no subscribers.
            if (TagEncountered is null)
                return null;
            
            var args = new TagHandledEventArgs(type, named, BaseStream);
            TagEncountered.Invoke(this, args);
            return args.Handled ? args.Result : null;
        }
        
    }
}