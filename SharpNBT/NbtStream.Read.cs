using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    public partial class NbtStream
    {
        /// <summary>
        /// Reads a <see cref="ByteTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ByteTag"/> instance.</returns>
        public ByteTag ReadByte(bool named = true)
        {
            var name = named ? ReadPrefixedString() : null;
            return new ByteTag(name, (byte)BaseStream.ReadByte());
        }

        /// <summary>
        /// Reads a <see cref="ShortTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ShortTag"/> instance.</returns>
        public ShortTag ReadShort(bool named = true)
        {
            var name = named ? ReadPrefixedString() : null;
            
            Span<byte> buffer = stackalloc byte[2];
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
            var name = named ? ReadPrefixedString() : null;
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
            var name = named ? ReadPrefixedString() : null;
            
            Span<byte> buffer = stackalloc byte[8];
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
            var name = named ? ReadPrefixedString() : null;
            
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
            var name = named ? ReadPrefixedString() : null;
            var buffer = new byte[sizeof(double)];
            BaseStream.Read(buffer, 0, sizeof(double));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            
            return new DoubleTag( name, BitConverter.ToDouble(buffer));
        }

        /// <summary>
        /// Reads a <see cref="StringTag"/> from the stream.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="StringTag"/> instance.</returns>
        public StringTag ReadString(bool named = true)
        {
            var name = named ? ReadPrefixedString() : null;
            var value = ReadPrefixedString();
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
            var name = named ? ReadPrefixedString() : null;
            var count = ReadInt32();
            var buffer = new byte[count];
            BaseStream.Read(buffer, 0, count);
            return new ByteArrayTag(name, new ReadOnlySpan<byte>(buffer));
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
            
            var name = named ? ReadPrefixedString() : null;
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
            
            var name = named ? ReadPrefixedString() : null;
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
            var name = named ? ReadPrefixedString() : null;
            var childType = ReadType();
            var count = ReadInt32();
            
            if (childType == TagType.End && count > 0)
                throw new FormatException("An EndTag is not a valid child type for a ListTag.");
            
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
            var name = named ? ReadPrefixedString() : null;
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
        /// Convenience method to read a tag and cast it with automatically.
        /// </summary>
        /// <param name="named">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
        /// <typeparam name="T">The tag type that is being read from the stream.</typeparam>
        /// <returns>The tag instance that was read from the stream.</returns>
        /// <remarks>This is typically only used when reading the top-level <see cref="CompoundTag"/> of a document where the type is already known.</remarks>
        public T ReadTag<T>(bool named = true) where T : Tag
        {
            return (T)ReadTag(named);
        }
        
        [NotNull] 
        protected Tag ReadTag(TagType type, bool named)
        {
            return type switch
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
        /// Reads a length-prefixed (unsigned short) UTF-8 string from the <see cref="BaseStream"/>.
        /// </summary>
        /// <returns>The string instance, or <see langword="null"/> if a length of <c>0</c> was specified.</returns>
        [CanBeNull]
        private string ReadPrefixedString()
        {
            Span<byte> lenBuffer = stackalloc byte[2];
            BaseStream.Read(lenBuffer);
            var len = BitConverter.ToUInt16(lenBuffer);
            if (BitConverter.IsLittleEndian)
                len = len.SwapEndian();
            
            if (len == 0)
                return null;
            
            Span<byte> buffer = stackalloc byte[len];
            BaseStream.Read(buffer);
            return Encoding.UTF8.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ReadInt32()
        {
            Span<byte> buffer = stackalloc byte[4];
            BaseStream.Read(buffer);
            var value = BitConverter.ToInt32(buffer);
            return BitConverter.IsLittleEndian ? value.SwapEndian() : value;
        }

    }
}