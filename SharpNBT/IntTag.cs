using System;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// A tag that contains a single 32-bit integer value.
    /// </summary>
    [PublicAPI]
    public class IntTag : Tag<int>
    {
        /// <summary>
        /// Gets or sets the value of this tag as an unsigned value.
        /// </summary>
        /// <remarks>
        /// This is only a reinterpretation of the bytes, no actual conversion is performed.
        /// </remarks>
        [CLSCompliant(false)]
        public uint UnsignedValue
        {
            get => unchecked((uint)Value);
            set => Value = unchecked((int)value);
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="IntTag"/> class with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="value">The value to assign to this tag.</param>
        public IntTag([CanBeNull] string name, int value) : base(TagType.Int, name, value)
        {
        }
        
        /// <inheritdoc cref="IntTag(string,int)"/>
        [CLSCompliant(false)]
        public IntTag([CanBeNull] Tag parent, [CanBeNull] string name, uint value) : base(TagType.Int, name, unchecked((int) value))
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"TAG_Int({PrettyName}): {Value}";
        
        /// <summary>
        /// Implicit conversion of this tag to a <see cref="int"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="int"/>.</returns>
        public static implicit operator int(IntTag tag) => tag.Value;
        
        /// <summary>
        /// Implicit conversion of this tag to a <see cref="uint"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="uint"/>.</returns>
        [CLSCompliant(false)]
        public static implicit operator uint(IntTag tag) => unchecked((uint)tag.Value);
    }
}