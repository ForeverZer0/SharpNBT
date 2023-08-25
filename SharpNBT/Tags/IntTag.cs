using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag containing a signed 32-bit integer.
/// </summary>
[PublicAPI]
public class IntTag : NumericTag<int>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Int;
    
    /// <summary>
    /// Gets the value of the tag as an unsigned value.
    /// </summary>
    [CLSCompliant(false)]
    public uint UnsignedValue => unchecked((uint)Value);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="IntTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public IntTag(string? name, int value) : base(name, value)
    {
    }
    
    /// <inheritdoc cref="IntTag(string?,int)"/>
    [CLSCompliant(false)]
    public IntTag(string? name, uint value) : base(name, Unsafe.As<uint, int>(ref value))
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Int({PrettyName}): {Value}";
    
    /// <summary>
    /// Implicit conversion of a an <see cref="IntTag"/> to a <see cref="uint"/>.
    /// </summary>
    /// <param name="tag">The <see cref="IntTag"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="uint"/>.</returns>
    [CLSCompliant(false)]
    public static implicit operator uint(IntTag tag) => unchecked((uint)tag.Value);
}