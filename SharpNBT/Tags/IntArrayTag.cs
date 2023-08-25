using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag that containing a contiguous array of signed 32-bit integers.
/// </summary>
[PublicAPI]
public class IntArrayTag : ArrayTag<int>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.IntArray;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="IntArrayTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public IntArrayTag(string? name, int[] value) : base(name, value)
    {
    }
    
    /// <inheritdoc cref="IntArrayTag(string?,int[])"/>
    public IntArrayTag(string? name, ReadOnlySpan<int> value) : base(name, value.ToArray())
    {
    }
    
    /// <inheritdoc cref="IntArrayTag(string?,int[])"/>
    public IntArrayTag(string? name, IEnumerable<int> value) : base(name, value.ToArray())
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_IntArray({PrettyName}): {Count} {word}";
    }
}