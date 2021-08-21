using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    public partial class NbtStream
    {
        /// <summary>
        /// Reads a <see cref="ByteTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ByteTag"/> instance.</returns>
        public new ByteTag ReadByte()
        {
            var name = named ? ReadPrefixedString() : null;
            return new ByteTag(name, (byte)BaseStream.ReadByte());
        }

        /// <summary>
        /// Reads a <see cref="ShortTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ShortTag"/> instance.</returns>
        public ShortTag ReadShort()
        {
            var name = named ? ReadPrefixedString() : null;
            return new ShortTag(name, BitConverter.ToInt16(ReadNumber(sizeof(short))));
        }

        /// <summary>
        /// Reads a <see cref="IntTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="IntTag"/> instance.</returns>
        public IntTag ReadInt()
        {
            var name = named ? ReadPrefixedString() : null;
            return new IntTag(name, BitConverter.ToInt32(ReadNumber(sizeof(int))));
        }
        
        /// <summary>
        /// Reads a <see cref="LongTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="LongTag"/> instance.</returns>
        public LongTag ReadLong()
        {
            var name = named ? ReadPrefixedString() : null;
            return new LongTag(name, BitConverter.ToInt64(ReadNumber(sizeof(long))));
        }

        /// <summary>
        /// Reads a <see cref="FloatTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="FloatTag"/> instance.</returns>
        public FloatTag ReadFloat()
        {
            var name = named ? ReadPrefixedString() : null;
            return new FloatTag(name, BitConverter.ToSingle(ReadNumber(sizeof(float))));
        }

        /// <summary>
        /// Reads a <see cref="DoubleTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="DoubleTag"/> instance.</returns>
        public DoubleTag ReadDouble()
        {
            var name = named ? ReadPrefixedString() : null;
            return new DoubleTag( name, BitConverter.ToDouble(ReadNumber(sizeof(double))));
        }

        /// <summary>
        /// Reads a <see cref="StringTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="StringTag"/> instance.</returns>
        public StringTag ReadString()
        {
            var name = named ? ReadPrefixedString() : null;
            var value = ReadPrefixedString();
            return new StringTag(name, value);
        }
        
        /// <summary>
        /// Reads a <see cref="ByteArrayTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ByteArrayTag"/> instance.</returns>
        public ByteArrayTag ReadByteArray()
        {
            var name = named ? ReadPrefixedString() : null;
            var count = BitConverter.ToInt32(ReadNumber(sizeof(int)));
            var buffer = new byte[count];
            BaseStream.Read(buffer, 0, count);
            return new ByteArrayTag(name, buffer);
        }
        
        /// <summary>
        /// Reads a <see cref="IntArrayTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="IntArrayTag"/> instance.</returns>
        public IntArrayTag ReadIntArray()
        {
            const int INT_SIZE = sizeof(int);
            
            var name = named ? ReadPrefixedString() : null;
            var count = BitConverter.ToInt32(ReadNumber(sizeof(int)));
            var buffer = new byte[count * INT_SIZE];
            BaseStream.Read(buffer, 0, count * INT_SIZE);

            var values = new int[count];
            var offset = 0;

            if (BitConverter.IsLittleEndian)
            {
                for (var i = 0; i < count; i++, offset += INT_SIZE)
                {
                    Array.Reverse(buffer, offset, INT_SIZE);
                    values[i] = BitConverter.ToInt32(buffer, offset);
                }
            }
            else
            {
                for (var i = 0; i < count; i++, offset += INT_SIZE)
                    values[i] = BitConverter.ToInt32(buffer, offset);
            }
            
            return new IntArrayTag(name, values);
        }
        
        /// <summary>
        /// Reads a <see cref="LongArrayTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="LongArrayTag"/> instance.</returns>
        public LongArrayTag ReadLongArray()
        {
            const int LONG_SIZE = sizeof(long);
            
            var name = named ? ReadPrefixedString() : null;
            var count = BitConverter.ToInt32(ReadNumber(sizeof(int)));
            var buffer = new byte[count * LONG_SIZE];
            BaseStream.Read(buffer, 0, count * LONG_SIZE);

            var values = new long[count];
            var offset = 0;

            if (BitConverter.IsLittleEndian)
            {
                for (var i = 0; i < count; i++, offset += LONG_SIZE)
                {
                    Array.Reverse(buffer, offset, LONG_SIZE);
                    values[i] = BitConverter.ToInt64(buffer, offset);
                }
            }
            else
            {
                for (var i = 0; i < count; i++, offset += LONG_SIZE)
                {
                    values[i] = BitConverter.ToInt64(buffer, offset);
                }
            }
            
            return new LongArrayTag(name, values);
        }
        
        /// <summary>
        /// Reads a <see cref="ListTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="ListTag"/> instance.</returns>
        public ListTag ReadList()
        {
            var name = named ? ReadPrefixedString() : null;
            var childType = ReadType();
            var count = BitConverter.ToInt32(ReadNumber(sizeof(int)));
            var list = new ListTag(name, childType);

            var previous = named;
            named = false;
            while (count-- > 0)
            {
                list.Add(ReadTag(childType));
            }
            named = previous;

            return list;
        }

        /// <summary>
        /// Reads a <see cref="CompoundTag"/> from the stream.
        /// </summary>
        /// <remarks>It is assumed that the stream is positioned at the beginning of the tag payload.</remarks>
        /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
        public CompoundTag ReadCompound()
        {
            var name = named ? ReadPrefixedString() : null;
            var compound = new CompoundTag(name);
            var previous = named;
            named = true;
            while (true)
            {
                var type = ReadType();
                if (type == TagType.End)
                    break;
                compound.Add(ReadTag(type));
            }

            named = previous;

            return compound;
        }
        
        /// <summary>
        /// Reads a <see cref="Tag"/> from the current position in the stream. 
        /// </summary>
        /// <returns></returns>
        public Tag ReadTag()
        {
            var type = ReadType();
            return ReadTag(type);
        }

        public T ReadTag<T>() where T : Tag
        {
            return (T)ReadTag();
        }
        
        protected Tag ReadTag(TagType type)
        {
            return type switch
            {
                TagType.End => new EndTag(),
                TagType.Byte => ReadByte(),
                TagType.Short => ReadShort(),
                TagType.Int => ReadInt(),
                TagType.Long => ReadLong(),
                TagType.Float => ReadFloat(),
                TagType.Double => ReadDouble(),
                TagType.ByteArray => ReadByteArray(),
                TagType.String => ReadString(),
                TagType.List => ReadList(),
                TagType.Compound => ReadCompound(),
                TagType.IntArray => ReadIntArray(),
                TagType.LongArray => ReadLongArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public TagType ReadType()
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
        protected string ReadPrefixedString()
        {
            var len = BitConverter.ToUInt16(ReadNumber(sizeof(ushort)));
            if (len == 0)
                return null;
            
            Span<byte> buffer = stackalloc byte[len];
            BaseStream.Read(buffer);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Reads <paramref name="count"/> bytes from the stream, reversing them if necessary to ensure proper endian format.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>An array of bytes that represent the number.</returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected byte[] ReadNumber(int count)
        {
            var buffer = new byte[count];
            BaseStream.Read(buffer, 0, buffer.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return buffer;
        }
    }
}