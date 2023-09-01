using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag that whose value is a contiguous sequence of 32-bit integers.
/// </summary>
[PublicAPI]
public class IntArrayTag : ArrayTag<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="capacity">The capacity of the array.</param>
    public IntArrayTag(string? name, int capacity) : base(TagType.IntArray, name, new int[capacity])
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
    public IntArrayTag(string? name, IEnumerable<int> values) : base(TagType.IntArray, name, values.ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public IntArrayTag(string? name, ReadOnlySpan<int> values) : base(TagType.IntArray, name, values.ToArray())
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
        return $"TAG_Int_Array({PrettyName}): [{Count} {word}]";
    }

    /// <inheritdoc />
    public override string Stringify(bool named = true) => Stringify(named, 'I', null);
}