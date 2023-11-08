using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag that whose value is a contiguous sequence of 64-bit integers.
/// </summary>
[PublicAPI]
public class LongArrayTag : ArrayTag<long>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LongArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="capacity">The capacity of the array.</param>
    public LongArrayTag(string? name, int capacity) : base(TagType.LongArray, name, new long[capacity])
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="LongArrayTag"/> with the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public LongArrayTag(string? name, long[] values) : base(TagType.LongArray, name, values.ToArray())
    {
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="LongArrayTag"/> with the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public LongArrayTag(string? name, IEnumerable<long> values) : base(TagType.LongArray, name, values.ToArray())
    {
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="LongArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public LongArrayTag(string? name, ReadOnlySpan<long> values) : base(TagType.LongArray, name, values.ToArray())
    {
    }
    
    /// <inheritdoc />
    protected internal override void WriteJson(Utf8JsonWriter writer, bool named = true)
    {
        if (named && Name != null)
        {
            writer.WriteStartArray(Name);
        }
        else
        {
            writer.WriteStartArray();
        }
        
        for (var i = 0; i < Count; i++)
            writer.WriteNumberValue(this[i]);
        writer.WriteEndArray();
    }
        
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        var word = Count == 1 ? Strings.WordElement : Strings.WordElements;
        return $"TAG_Long_Array({WarpedName}): [{Count} {word}]";
    }
    //public override string ToWarppedString()
    //{
    //    var word = Count == 1 ? Strings.WordElement : Strings.WordElements;
    //    return $"[{Count} {word}]";
    //}

    /// <inheritdoc />
    public override string Stringify(bool named = true) => Stringify(named, 'L', 'l');
}