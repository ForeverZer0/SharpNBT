using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using JetBrains.Annotations;

[assembly: CLSCompliant(true)]

namespace SharpNBT
{
    /// <summary>
    /// Abstract base class that all NBT tags inherit from.
    /// </summary>
    [PublicAPI][DataContract][KnownType("GetKnownTypes")]
    public abstract class Tag
    {
        private static IEnumerable<Type> GetKnownTypes()
        {
            return new[]
            {
                typeof(TagType),
                typeof(Tag<>),
                typeof(EnumerableTag<>),
                typeof(TagContainer),
                typeof(ByteTag),
                typeof(ShortTag),
                typeof(IntTag),
                typeof(LongTag),
                typeof(FloatTag),
                typeof(DoubleTag),
                typeof(StringTag),
                typeof(ByteArrayTag),
                typeof(IntArrayTag),
                typeof(LongArrayTag),
                typeof(ListTag),
                typeof(CompoundTag)
            };
        }
        
        /// <summary>
        /// Text applied in a pretty-print sting when a tag has no defined <see cref="Name"/> value.
        /// </summary>
        protected const string NO_NAME = "None";
        
        /// <summary>
        /// Gets a constant describing the NBT type this object represents.
        /// </summary>
        [DataMember(IsRequired = true, Name = "type", Order = 0)]
        public TagType Type { get; private set; }
        
        /// <summary>
        /// Gets the parent <see cref="Tag"/> this object is a child of.
        /// </summary>
        [CanBeNull]
        public Tag Parent { get; internal set; }
        
        /// <summary>
        /// Gets the name assigned to this <see cref="Tag"/>.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "name")]
        [CanBeNull]
        public string Name { get; set; }

        /// <summary>
        /// Initialized a new instance of the <see cref="Tag"/> class.
        /// </summary>
        /// <param name="type">A constant describing the NBT type for this tag.</param>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        protected Tag(TagType type, [CanBeNull] string name)
        {
            Type = type;
            Name = name;
        }

        /// <summary>
        /// Writes this tag as a formatted string to the given <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">A <see cref="StringBuilder"/> instance to write to.</param>
        /// <param name="level">The current indent depth to write at.</param>
        /// <param name="indent">The string to use for indents.</param>
        protected internal abstract void PrettyPrinted([NotNull] StringBuilder buffer, int level, [NotNull] string indent);
        
        /// <summary>
        /// Gets the name of the object as a human-readable quoted string, or a default name to indicate it has no name when applicable.
        /// </summary>
        protected internal string PrettyName => Name is null ? "None" : $"\"{Name}\"";
        
        /// <summary>
        /// Gets a representation of this <see cref="Tag"/> as a JSON string.
        /// </summary>
        /// <param name="pretty">Flag indicating if formatting should be applied to make the string human-readable.</param>
        /// <param name="indent">When <paramref name="pretty"/> is <see lawnword="true"/>, indicates the indent characters(s) to use.</param>
        /// <returns>A JSON string describing this object.</returns>
        public string ToJsonString(bool pretty = false, string indent = "    ")
        {
            var settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
                EmitTypeInformation = EmitTypeInformation.Never,
                KnownTypes = GetKnownTypes()
            };
            var serializer = new DataContractJsonSerializer(typeof(Tag), settings);
            using var stream = new MemoryStream();
            if (pretty)
            {
                using var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false, true, indent);
                serializer.WriteObject(writer, this);
                writer.Flush();
            }
            else
            {
                serializer.WriteObject(stream, this);
            }
            stream.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        
        public string ToXmlString()
        {
            var settings = new DataContractSerializerSettings
            {
                KnownTypes = GetKnownTypes()
            };
            var serializer = new DataContractSerializer(typeof(Tag), settings);
            using var stream = new MemoryStream();
           
            serializer.WriteObject(stream, this);
            stream.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
    
    /// <summary>
    /// Abstract base class for <see cref="Tag"/> types that contain a single primitive value.
    /// </summary>
    /// <typeparam name="T">The type of the value the tag represents.</typeparam>
    [PublicAPI][DataContract]
    public abstract class Tag<T> : Tag
    {
        /// <summary>
        /// Gets or sets the value of the tag.
        /// </summary>
        [DataMember(IsRequired = false, Name = "value")]
        public T Value { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="DoubleTag"/> class with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="type">A constant describing the NBT type for this tag.</param>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="value">The value to assign to this tag.</param>
        protected Tag(TagType type, [CanBeNull] string name, T value) : base(type, name)
        {
            Value = value;
        }

        /// <inheritdoc cref="Tag.PrettyPrinted(StringBuilder,int,string)"/>
        protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
        {
            for (var i = 0; i < level; i++)
                buffer.Append(indent);
            buffer.AppendLine(ToString());
        }
    }
}