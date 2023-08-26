using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag containing a signed 64-bit integer.
/// </summary>
[PublicAPI]
public class LongTag : NumericTag<long>, IValueTag<long>
{
    /// <inheritdoc />
    static TagType ITag.Type => Type;

    /// <inheritdoc cref="ITag.Type"/>
    public static TagType Type => TagType.Long;

    /// <summary>
    /// Gets the value of the tag as an unsigned value.
    /// </summary>
    [CLSCompliant(false)]
    public ulong UnsignedValue => unchecked((ulong)Value);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LongTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public LongTag(string? name, long value) : base(name, value)
    {
    }
    
    /// <inheritdoc cref="LongTag(string?,long)"/>
    [CLSCompliant(false)]
    public LongTag(string? name, ulong value) : base(name, Unsafe.As<ulong, long>(ref value))
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Long({PrettyName}): {Value}";
    
    /// <summary>
    /// Implicit conversion of a an <see cref="LongTag"/> to a <see cref="ulong"/>.
    /// </summary>
    /// <param name="tag">The <see cref="LongTag"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="ulong"/>.</returns>
    [CLSCompliant(false)]
    public static implicit operator ulong(LongTag tag) => unchecked((ulong)tag.Value);
}