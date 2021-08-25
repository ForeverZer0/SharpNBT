using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// A tag that contains a single 16-bit integer value.
    /// </summary>
    [PublicAPI][Serializable]
    public class ShortTag : Tag<short>
    {
        /// <summary>
        /// Gets or sets the value of this tag as an unsigned value.
        /// </summary>
        /// <remarks>
        /// This is only a reinterpretation of the bytes, no actual conversion is performed.
        /// </remarks>
        [CLSCompliant(false)]
        public ushort UnsignedValue
        {
            get => unchecked((ushort)Value);
            set => Value = unchecked((short)value);
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="ShortTag"/> class with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="value">The value to assign to this tag.</param>
        public ShortTag([CanBeNull] string name, short value) : base(TagType.Short, name, value)
        {
        }
        
        /// <inheritdoc cref="ShortTag(string,short)"/>
        [CLSCompliant(false)]
        public ShortTag([CanBeNull] string name, ushort value) : base(TagType.Short, name, unchecked((short) value))
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"TAG_Short({PrettyName}): {Value}";
        
        /// <summary>
        /// Implicit conversion of this tag to a <see cref="short"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="short"/>.</returns>
        public static implicit operator short(ShortTag tag) => tag.Value;
        
        /// <summary>
        /// Implicit conversion of this tag to a <see cref="ushort"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="ushort"/>.</returns>
        [CLSCompliant(false)]
        public static implicit operator ushort(ShortTag tag) => unchecked((ushort)tag.Value);
    }
}