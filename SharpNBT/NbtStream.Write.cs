using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpNBT
{
    public partial class NbtStream
    {
        public void WriteType(Tag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
        }

        public void WriteByte(ByteTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);
            BaseStream.WriteByte(tag.Value);
        }

        public void WriteShort(ShortTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);
            WriteNumber(BitConverter.GetBytes(tag.Value));
        }

        public void WriteInt(IntTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);
            WriteNumber(BitConverter.GetBytes(tag.Value));
        }

        public void WriteLong(LongTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);
            WriteNumber(BitConverter.GetBytes(tag.Value));
        }

        public void WriteFloat(FloatTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);
            WriteNumber(BitConverter.GetBytes(tag.Value));
        }

        public void WriteDouble(DoubleTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);
            WriteNumber(BitConverter.GetBytes(tag.Value));
        }

        public void WriteString(StringTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);
            WriteString(tag.Value);
        }
        
        public void WriteByteArray(ByteArrayTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);

            WriteNumber(BitConverter.GetBytes(tag.Count));
            BaseStream.Write(tag.ToArray(), 0, tag.Count);
        }

        public void WriteIntArray(IntArrayTag tag)
        {
            const int INT_SIZE = sizeof(int);
            
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);

            WriteNumber(BitConverter.GetBytes(tag.Count));
            var values = tag.ToArray();
            var buffer = new byte[tag.Count * INT_SIZE];
            byte[] bits;
            
            var offset = 0;
            if (BitConverter.IsLittleEndian)
            {
                for (var i = 0; i < tag.Count; i++, offset += INT_SIZE)
                {
                    bits = BitConverter.GetBytes(values[i]);
                    Array.Reverse(bits, 0, INT_SIZE);
                    bits.CopyTo(buffer, offset);
                }   
            }
            else
            {
                for (var i = 0; i < tag.Count; i++, offset += INT_SIZE)
                {
                    bits = BitConverter.GetBytes(values[i]);
                    bits.CopyTo(buffer, offset);
                }
            }

            BaseStream.Write(buffer, 0, buffer.Length);
        }

        public void WriteLongArray(LongArrayTag tag)
        {
            const int LONG_SIZE = sizeof(long);
            
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);

            WriteNumber(BitConverter.GetBytes(tag.Count));
            var values = tag.ToArray();
            var buffer = new byte[tag.Count * LONG_SIZE];
            byte[] bits;
            
            var offset = 0;
            if (BitConverter.IsLittleEndian)
            {
                for (var i = 0; i < tag.Count; i++, offset += LONG_SIZE)
                {
                    bits = BitConverter.GetBytes(values[i]);
                    Array.Reverse(bits, 0, LONG_SIZE);
                    bits.CopyTo(buffer, offset);
                }   
            }
            else
            {
                for (var i = 0; i < tag.Count; i++, offset += LONG_SIZE)
                {
                    bits = BitConverter.GetBytes(values[i]);
                    bits.CopyTo(buffer, offset);
                }
            }

            BaseStream.Write(buffer, 0, buffer.Length);
        }

        public void WriteList(ListTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);

            BaseStream.WriteByte((byte) tag.ChildType);
            WriteNumber(BitConverter.GetBytes(tag.Count));

            nameStack.Push(false);
            foreach (var child in tag)
                WriteTag(child);
            nameStack.Pop();
        }
        
        public void WriteCompound(CompoundTag tag)
        {
            BaseStream.WriteByte((byte) tag.Type);
            if (nameStack.Peek())
                WriteString(tag.Name);

            nameStack.Push(true);
            foreach (var child in tag)
            {
                child.Parent = tag;
                WriteTag(child);
            }
            nameStack.Pop();
            
            WriteEndTag();
        }
        
        public void WriteEndTag() => BaseStream.WriteByte((byte) TagType.End);
        
        public void WriteTag(Tag tag)
        {
            switch (tag.Type)
            {
                case TagType.End: 
                    WriteEndTag();
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
        private void WriteNumber(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            BaseStream.Write(data, 0, data.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteNumber(BitConverter.GetBytes((ushort) 0));
            }
            else
            {
                var utf8 = Encoding.UTF8.GetBytes(value);
                WriteNumber(BitConverter.GetBytes((ushort) utf8.Length));
                BaseStream.Write(utf8, 0, utf8.Length);
            }
        }
    }
}