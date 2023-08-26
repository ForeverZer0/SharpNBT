using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag that whose value is a contiguous sequence of 32-bit integers.
/// </summary>
[PublicAPI][Serializable]
public class IntArrayTag : EnumerableTag<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    public IntArrayTag(string? name) : base(TagType.IntArray, name)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="IntArrayTag"/> with the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public IntArrayTag(string? name, int[] values) : base(TagType.IntArray, name, values)
    {
    }
        
        
    /// <summary>
    /// Initializes a new instance of the <see cref="IntArrayTag"/> with the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public IntArrayTag(string? name, IEnumerable<int> values) : base(TagType.IntArray, name, values)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public IntArrayTag(string? name, ReadOnlySpan<int> values) : base(TagType.IntArray, name, values)
    {
    }
        
    /// <summary>
    /// Required constructor for ISerializable implementation.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to describing this instance.</param>
    /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
    protected IntArrayTag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
        
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        var word = Count == 1 ? Strings.WordElement : Strings.WordElements;
        return $"TAG_Int_Array({PrettyName}): [{Count} {word}]";
    }
        
    /// <summary>
    /// Gets the <i>string</i> representation of this NBT tag (SNBT).
    /// </summary>
    /// <returns>This NBT tag in SNBT format.</returns>
    /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
    public override string Stringify() => $"{StringifyName}[I;{string.Join(',', this)}]";
}