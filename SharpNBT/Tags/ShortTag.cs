using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag containing a signed 16-bit integer.
/// </summary>
[PublicAPI]
public class ShortTag : NumericTag<short>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Short;
    
    /// <summary>
    /// Gets the value of the tag as an unsigned value.
    /// </summary>
    [CLSCompliant(false)]
    public ushort UnsignedValue => unchecked((ushort)Value);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ShortTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public ShortTag(string? name, short value) : base(name, value)
    {
    }
    
    /// <inheritdoc cref="ShortTag(string?,short)"/>
    /// <remarks>The use of an <see cref="int"/> is for convenience only.</remarks>
    public ShortTag(string? name, int value) : base(name, unchecked((short)(value & 0xFFFF)))
    {
    }
    
    /// <inheritdoc cref="ShortTag(string?,short)"/>
    [CLSCompliant(false)]
    public ShortTag(string? name, ushort value) : base(name, Unsafe.As<ushort, short>(ref value))
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Short({PrettyName}): {Value}";
    
    /// <summary>
    /// Implicit conversion of a an <see cref="ShortTag"/> to a <see cref="ushort"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ShortTag"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="ushort"/>.</returns>
    [CLSCompliant(false)]
    public static implicit operator ushort(ShortTag tag) => unchecked((ushort)tag.Value);
}