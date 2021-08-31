using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// A tag that whose value is a contiguous sequence of 8-bit integers.
    /// </summary>
    /// <remarks>
    /// While this class uses the CLS compliant <see cref="byte"/> (0..255), the NBT specification uses a signed value with a range of -128..127, so ensure
    /// the bits are equivalent for your values.
    /// </remarks>
    [PublicAPI][Serializable]
    public class ByteArrayTag : EnumerableTag<byte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayTag"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection of values to include in this tag.</param>
        public ByteArrayTag([CanBeNull] string name, byte[] values) : base(TagType.ByteArray, name, values)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayTag"/> with the specified <paramref name="values"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection of values to include in this tag.</param>
        public ByteArrayTag([CanBeNull] string name, [NotNull] IEnumerable<byte> values) : base(TagType.ByteArray, name, values)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayTag"/> with the specified <paramref name="values"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection of values to include in this tag.</param>
        public ByteArrayTag([CanBeNull] string name, ReadOnlySpan<byte> values) : base(TagType.ByteArray, name, values)
        {
        }
        
        /// <summary>
        /// Required constructor for ISerializable implementation.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to describing this instance.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        protected ByteArrayTag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            var word = Count == 1 ? Strings.WordElement : Strings.WordElements;
            return $"TAG_Byte_Array({PrettyName}): [{Count} {word}]";
        }

        /// <summary>
        /// Gets the <i>string</i> representation of this NBT tag (SNBT).
        /// </summary>
        /// <returns>This NBT tag in SNBT format.</returns>
        /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
        public override string Stringify()
        {
            var values = new string[Count];
            for (var i = 0; i < Count; i++)
                values[i] = $"{this[i]}b";
            return $"{StringifyName}[B;{string.Join(',', values)}]";
        }
    }
}