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
    [PublicAPI][Serializable]
    public abstract class Tag : IEquatable<Tag>, ISerializable
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
        public TagType Type { get; private set; }
        
        /// <summary>
        /// Gets the parent <see cref="Tag"/> this object is a child of.
        /// </summary>
        [CanBeNull]
        public Tag Parent { get; internal set; }
        
        /// <summary>
        /// Gets the name assigned to this <see cref="Tag"/>.
        /// </summary>
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

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1.Equals?view=netstandard-2.1">`IEquatable.Equals` on docs.microsoft.com</a></footer>
        public bool Equals(Tag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Name == other.Name;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.Equals?view=netstandard-2.1">`Object.Equals` on docs.microsoft.com</a></footer>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tag)obj);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.GetHashCode?view=netstandard-2.1">`Object.GetHashCode` on docs.microsoft.com</a></footer>
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return ((int)Type * 373) ^ (Name != null ? Name.GetHashCode() : 0);
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        protected Tag(SerializationInfo info, StreamingContext context)
        {
            Type = (TagType) info.GetByte("type");
            Name = info.GetString("name");
        }

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", (byte) Type);
            info.AddValue("name", Name);
        }

        public static bool operator ==(Tag left, Tag right) => Equals(left, right);

        public static bool operator !=(Tag left, Tag right) => !Equals(left, right);
    }
    
    /// <summary>
    /// Abstract base class for <see cref="Tag"/> types that contain a single primitive value.
    /// </summary>
    /// <typeparam name="T">The type of the value the tag represents.</typeparam>
    [PublicAPI][Serializable]
    public abstract class Tag<T> : Tag, IEquatable<Tag<T>>
    {
        /// <summary>
        /// Gets or sets the value of the tag.
        /// </summary>
        public T Value { get; set; }
        
        protected Tag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Value = (T)info.GetValue("value", typeof(T));
        }

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("value", Value, typeof(T));
        }

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

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1.Equals?view=netstandard-2.1">`IEquatable.Equals` on docs.microsoft.com</a></footer>
        public bool Equals(Tag<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.Equals?view=netstandard-2.1">`Object.Equals` on docs.microsoft.com</a></footer>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Tag<T>)obj);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.GetHashCode?view=netstandard-2.1">`Object.GetHashCode` on docs.microsoft.com</a></footer>
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (base.GetHashCode() * 421) ^ EqualityComparer<T>.Default.GetHashCode(Value);
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        public static bool operator ==(Tag<T> left, Tag<T> right) => Equals(left, right);

        public static bool operator !=(Tag<T> left, Tag<T> right) => !Equals(left, right);
    }
}