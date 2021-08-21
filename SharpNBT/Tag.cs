using System;
using System.Text;
using JetBrains.Annotations;

[assembly: CLSCompliant(true)]

namespace SharpNBT
{
    /// <summary>
    /// Abstract base class that all NBT tags inherit from.
    /// </summary>
    [PublicAPI]
    public abstract class Tag
    {
        /// <summary>
        /// Text applied in a pretty-print sting when a tag has no defined <see cref="Name"/> value.
        /// </summary>
        protected const string NO_NAME = "None";
        
        /// <summary>
        /// Gets a constant describing the NBT type this object represents.
        /// </summary>
        public TagType Type { get; }
        
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

        protected internal abstract void PrettyPrinted(StringBuilder buffer, int level, string indent);
        
        protected internal string PrettyName => Name is null ? "None" : $"\"{Name}\"";
    }

    /// <summary>
    /// Abstract base class for <see cref="Tag"/> types that contain a single primitive value.
    /// </summary>
    /// <typeparam name="T">The type of the value the tag represents.</typeparam>
    [PublicAPI]
    public abstract class Tag<T> : Tag
    {
        /// <summary>
        /// Gets or sets the value of the tag.
        /// </summary>
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
        
        protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
        {
            for (var i = 0; i < level; i++)
                buffer.Append(indent);
            buffer.AppendLine(ToString());
        }
    }

}