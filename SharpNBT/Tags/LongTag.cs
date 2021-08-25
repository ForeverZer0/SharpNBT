using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// A tag that contains a single 64-bit integer value.
    /// </summary>
    [PublicAPI][Serializable]
    public class LongTag : Tag<long>
    {
        /// <summary>
        /// Gets or sets the value of this tag as an unsigned value.
        /// </summary>
        /// <remarks>
        /// This is only a reinterpretation of the bytes, no actual conversion is performed.
        /// </remarks>
        [CLSCompliant(false)]
        public ulong UnsignedValue
        {
            get => unchecked((ulong)Value);
            set => Value = unchecked((long)value);
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="LongTag"/> class with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="value">The value to assign to this tag.</param>
        public LongTag([CanBeNull] string name, long value) : base(TagType.Long, name, value)
        {
        }
        
        /// <inheritdoc cref="LongTag(string,long)"/>
        [CLSCompliant(false)]
        public LongTag([CanBeNull] string name, ulong value) : base(TagType.Long, name, unchecked((long) value))
        {
        }
        
        /// <summary>
        /// Required constructor for ISerializable implementation.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to describing this instance.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        protected LongTag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"TAG_Long({PrettyName}): {Value}";
        
        /// <summary>
        /// Implicit conversion of this tag to a <see cref="long"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="long"/>.</returns>
        public static implicit operator long(LongTag tag) => tag.Value;
        
        /// <summary>
        /// Implicit conversion of this tag to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="ulong"/>.</returns>
        [CLSCompliant(false)]
        public static implicit operator ulong(LongTag tag) => unchecked((ulong)tag.Value);
    }
}