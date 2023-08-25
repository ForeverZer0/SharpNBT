using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag that containing a contiguous array of unsigned 8-bit integers.
/// </summary>
[PublicAPI]
public class ByteArrayTag : ArrayTag<byte>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.ByteArray;

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the tag's values.
    /// </summary>
    [CLSCompliant(false)]
    public Span<sbyte> SignedSpan => MemoryMarshal.Cast<byte, sbyte>(Span);

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArrayTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public ByteArrayTag(string? name, byte[] value) : base(name, value)
    {
    }
    
    /// <inheritdoc cref="ByteArrayTag(string?,byte[])"/>
    public ByteArrayTag(string? name, ReadOnlySpan<byte> value) : base(name, value.ToArray())
    {
    }
    
    /// <inheritdoc cref="ByteArrayTag(string?,byte[])"/>
    public ByteArrayTag(string? name, IEnumerable<byte> value) : base(name, value.ToArray())
    {
    }
    
    /// <inheritdoc cref="ByteArrayTag(string?,byte[])"/>
    [CLSCompliant(false)]
    public ByteArrayTag(string? name, sbyte[] value) : base(name, MemoryMarshal.Cast<sbyte, byte>(value).ToArray())
    {
    }
    
    /// <inheritdoc cref="ByteArrayTag(string?,byte[])"/>
    [CLSCompliant(false)]
    public ByteArrayTag(string? name, ReadOnlySpan<sbyte> value) : base(name, MemoryMarshal.Cast<sbyte, byte>(value).ToArray())
    {
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_ByteArray({PrettyName}): {Count} {word}";
    }
}