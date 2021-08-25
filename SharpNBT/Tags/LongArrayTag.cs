using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// A tag that whose value is a contiguous sequence of 64-bit integers.
    /// </summary>
    [PublicAPI][Serializable]
    public class LongArrayTag : EnumerableTag<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LongArrayTag"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        public LongArrayTag([CanBeNull] string name) : base(TagType.LongArray, name)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LongArrayTag"/> with the specified <paramref name="values"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection of values to include in this tag.</param>
        public LongArrayTag([CanBeNull] string name, [NotNull] long[] values) : base(TagType.LongArray, name, values)
        {
        }
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LongArrayTag"/> with the specified <paramref name="values"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection of values to include in this tag.</param>
        public LongArrayTag([CanBeNull] string name, [NotNull] IEnumerable<long> values) : base(TagType.LongArray, name, values)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LongArrayTag"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection of values to include in this tag.</param>
        public LongArrayTag([CanBeNull] string name, ReadOnlySpan<long> values) : base(TagType.LongArray, name, values)
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            var word = Count == 1 ? "element" : "elements";
            return $"TAG_Long_Array({PrettyName}): [{Count} {word}]";
        }
    }
}