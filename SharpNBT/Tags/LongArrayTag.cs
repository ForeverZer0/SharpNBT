using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag that containing a contiguous array of signed 64-bit integers.
/// </summary>
[PublicAPI]
public class LongArrayTag : ArrayTag<long>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.LongArray;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LongArrayTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public LongArrayTag(string? name, long[] value) : base(name, value)
    {
    }

    /// <inheritdoc cref="LongArrayTag(string?,long[])"/>
    public LongArrayTag(string? name, ReadOnlySpan<long> value) : base(name, value.ToArray())
    {
    }
    
    /// <inheritdoc cref="LongArrayTag(string?,long[])"/>
    public LongArrayTag(string? name, IEnumerable<long> value) : base(name, value.ToArray())
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_LongArray({PrettyName}): [{Count} {word}]";
    }
}