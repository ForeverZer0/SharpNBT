using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag containing an unsigned byte. The NBT specification uses a signed byte, but as a design decision, this
/// library uses an unsigned <see cref="byte"/>. The in-memory representation is not changed, merely how it is
/// interpreted. 
/// </summary>
/// <remarks>
/// If the range of a <see cref="sbyte"/> value is actually required, simply cast it or use the
/// <see cref="SignedValue"/> property.
/// </remarks>
[PublicAPI]
public class ByteTag : NumericTag<byte>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Byte;

    /// <summary>
    /// Gets the value in its native range, a signed <see cref="sbyte"/>.
    /// </summary>
    [CLSCompliant(false)]
    public sbyte SignedValue => unchecked((sbyte)Value);
    
    /// <summary>
    /// Gets the value of this tag as a <see cref="bool"/>.
    /// </summary>
    public bool Bool => Value != 0;
    
    /// <summary>
    /// Gets a flag indicating if this instance was created/parsed as a boolean rather than an integral value.
    /// </summary>
    public bool IsBool { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public ByteTag(string? name, byte value) : base(name, value)
    {
    }
    
    /// <inheritdoc cref="ByteTag(string?,byte)"/>
    /// <remarks>The use of an <see cref="int"/> is for convenience only.</remarks>
    public ByteTag(string? name, int value) : base(name, unchecked((byte)(value & 0xFF)))
    {
    }
    
    /// <inheritdoc cref="ByteTag(string?,byte)"/>
    [CLSCompliant(false)]
    public ByteTag(string? name, sbyte value) : base(name, Unsafe.As<sbyte, byte>(ref value))
    {
    }
    
    /// <inheritdoc cref="ByteTag(string?,byte)"/>
    public ByteTag(string? name, bool value) : base(name, value ? (byte) 1 : (byte)0)
    {
        IsBool = true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        object value = IsBool ? Bool : Value;
        return $"TAG_Byte({PrettyName}): {value}";
    }

    /// <summary>
    /// Implicit conversion of a an <see cref="ByteTag"/> to a <see cref="bool"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ByteTag"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="bool"/>.</returns>
    public static implicit operator bool(ByteTag tag) => tag.Value != 0;
    
    /// <summary>
    /// Implicit conversion of a an <see cref="ByteTag"/> to a <see cref="sbyte"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ByteTag"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="sbyte"/>.</returns>
    [CLSCompliant(false)]
    public static implicit operator sbyte(ByteTag tag) => unchecked((sbyte)tag.Value);
}