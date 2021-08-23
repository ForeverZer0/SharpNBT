using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    [PublicAPI][DataContract(Name = "byte_array")]
    public class ByteArrayTag : EnumerableTag<byte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayTag"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        public ByteArrayTag([CanBeNull] string name) : base(TagType.ByteArray, name)
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

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            var word = Count == 1 ? "element" : "elements";
            return $"TAG_Byte_Array({PrettyName}): [{Count} {word}]";
        }
    }
}