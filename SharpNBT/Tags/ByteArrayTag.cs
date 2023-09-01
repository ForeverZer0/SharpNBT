using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag that whose value is a contiguous sequence of 8-bit integers.
/// </summary>
/// <remarks>
/// While this class uses the CLS compliant <see cref="byte"/> (0..255), the NBT specification uses a signed value with a range of -128..127, so ensure
/// the bits are equivalent for your values.
/// </remarks>
[PublicAPI]
public class ByteArrayTag : ArrayTag<byte>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="capacity">The capacity of the array.</param>
    public ByteArrayTag(string? name, int capacity) : base(TagType.IntArray, name, new byte[capacity])
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArrayTag"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public ByteArrayTag(string? name, byte[] values) : base(TagType.ByteArray, name, values)
    {
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArrayTag"/> with the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public ByteArrayTag(string? name, IEnumerable<byte> values) : base(TagType.ByteArray, name, values.ToArray())
    {
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArrayTag"/> with the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection of values to include in this tag.</param>
    public ByteArrayTag(string? name, ReadOnlySpan<byte> values) : base(TagType.ByteArray, name, values.ToArray())
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
        return $"TAG_Byte_Array({PrettyName}): [{Count} {word}]";
    }
    
    /// <inheritdoc />
    public override string Stringify(bool named = true) => Stringify(named, 'B', 'b');
}