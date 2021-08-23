using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    public partial class NbtStream
    {
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
            BaseStream.Write(tag.Value.GetBytes(), 0, sizeof(short));
        }

        /// <summary>
        /// Writes a <see cref="IntTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="IntTag"/> instance to write.</param>
        public virtual void WriteInt(IntTag tag)
        {
            WriteTypeAndName(tag);
            BaseStream.Write(tag.Value.GetBytes(), 0, sizeof(int));
        }

        /// <summary>
        /// Writes a <see cref="LongTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="LongTag"/> instance to write.</param>
        public virtual void WriteLong(LongTag tag)
        {
            WriteTypeAndName(tag);
            BaseStream.Write(tag.Value.GetBytes(), 0, sizeof(long));
        }

        /// <summary>
        /// Writes a <see cref="FloatTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="FloatTag"/> instance to write.</param>
        public virtual void WriteFloat(FloatTag tag)
        {
            WriteTypeAndName(tag);
            BaseStream.Write(tag.Value.GetBytes(), 0, sizeof(float));
        }

        /// <summary>
        /// Writes a <see cref="DoubleTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="DoubleTag"/> instance to write.</param>
        public virtual void WriteDouble(DoubleTag tag)
        {
            WriteTypeAndName(tag);
            BaseStream.Write(tag.Value.GetBytes(), 0, sizeof(double));
        }

        /// <summary>
        /// Writes a <see cref="StringTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="StringTag"/> instance to write.</param>
        public virtual void WriteString(StringTag tag)
        {
            WriteTypeAndName(tag);
            WriteString(tag.Value);
        }
        
        /// <summary>
        /// Writes a <see cref="ByteArrayTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="ByteArrayTag"/> instance to write.</param>
        public virtual void WriteByteArray(ByteArrayTag tag)
        {
            WriteTypeAndName(tag);
            BaseStream.Write(tag.Count.GetBytes(), 0, sizeof(int));
            BaseStream.Write(tag.ToArray(), 0, tag.Count);
        }

        /// <summary>
        /// Writes a <see cref="IntArrayTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="IntArrayTag"/> instance to write.</param>
        public virtual void WriteIntArray(IntArrayTag tag)
        {
            WriteTypeAndName(tag);
            BaseStream.Write(tag.Count.GetBytes(), 0, sizeof(int));
            
            var values = new Span<int>(tag.ToArray());
            if (BitConverter.IsLittleEndian)
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
            BaseStream.Write(tag.Count.GetBytes(), 0, sizeof(int));

            var values = new Span<long>(tag.ToArray());
            if (BitConverter.IsLittleEndian)
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
            BaseStream.Write(tag.Count.GetBytes(), 0, sizeof(int));

            includeName.Push(false);
            foreach (var child in tag)
            {
                if (child.Name != null)
                    throw new FormatException("Tags that are children of a ListTag may not have named.");
                if (child.Type != tag.ChildType)
                    throw new FormatException("A ListTag may only contain a single type.");
                
                WriteTag(child);
            }
            includeName.Pop();
        }
        
        /// <summary>
        /// Writes a <see cref="CompoundTag"/> to the stream.
        /// </summary>
        /// <param name="tag">The <see cref="CompoundTag"/> instance to write.</param>
        public virtual void WriteCompound(CompoundTag tag)
        {
            WriteTypeAndName(tag);

            includeName.Push(true);
            foreach (var child in tag)
            {
                if (tag.Type == TagType.End)
                    break;
                
                child.Parent = tag;
                WriteTag(child);
            }
            
            BaseStream.WriteByte((byte) TagType.End);
            includeName.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public virtual void WriteEndTag([CanBeNull] EndTag tag = null) => BaseStream.WriteByte(0); 

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
                    throw new ArgumentOutOfRangeException(nameof(tag.Type), "Unknown tag type.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteTypeAndName(Tag tag)
        {
            if (tag.Parent is ListTag)
                return;

            BaseStream.WriteByte((byte) tag.Type);
            WriteString(tag.Name);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                BaseStream.Write(((ushort) 0).GetBytes(), 0, sizeof(ushort));
            }
            else
            {
                var utf8 = Encoding.UTF8.GetBytes(value);
                BaseStream.Write(((ushort) utf8.Length).GetBytes(), 0, sizeof(ushort));
                BaseStream.Write(utf8, 0, utf8.Length);
            }
        }
    }
}