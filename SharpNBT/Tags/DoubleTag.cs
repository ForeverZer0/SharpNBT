using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// A tag that contains a single IEEE-754 double-precision floating point number.
    /// </summary>
    [PublicAPI][Serializable]
    public class DoubleTag : Tag<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DoubleTag"/> class with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="value">The value to assign to this tag.</param>
        public DoubleTag([CanBeNull] string name, double value) : base(TagType.Double, name, value)
        {
        }
        
        /// <summary>
        /// Required constructor for ISerializable implementation.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to describing this instance.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        protected DoubleTag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"TAG_Double({PrettyName}): {Value}";
        
        /// <summary>
        /// Implicit conversion of this tag to a <see cref="double"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="double"/>.</returns>
        public static implicit operator double(DoubleTag tag) => tag.Value;
        
        /// <summary>
        /// Gets the <i>string</i> representation of this NBT tag (SNBT).
        /// </summary>
        /// <returns>This NBT tag in SNBT format.</returns>
        /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
        public override string Stringify() => $"{StringifyName}{Value:0.0}D";
    }
}