using System;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag that contains a single 8-bit integer value.
/// </summary>
/// <remarks>
/// While this class uses the CLS compliant <see cref="Byte"/> (0..255), the NBT specification uses a signed value with a range of -128..127. It is
/// recommended to use the <see cref="SignedValue"/> property if your language supports a signed 8-bit value, otherwise simply ensure the bits are
/// equivalent.
/// </remarks>
[PublicAPI][Serializable]
public class ByteTag : Tag<byte>
{
    /// <summary>
    /// Gets or sets the value of this tag as an unsigned value.
    /// </summary>
    /// <remarks>
    /// This is only a reinterpretation of the bytes, no actual conversion is performed.
    /// </remarks>
    [CLSCompliant(false)]
    public sbyte SignedValue
    {
        get => unchecked((sbyte)Value);
        set => Value = unchecked((byte)value);
    }
        
    /// <summary>
    /// Creates a new instance of the <see cref="ByteTag"/> class with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="value">The value to assign to this tag.</param>
    public ByteTag(string? name, byte value) : base(TagType.Byte, name, value)
    {
    }
        
    /// <inheritdoc cref="ByteTag(string,byte)"/>
    [CLSCompliant(false)]
    public ByteTag(string? name, sbyte value) : base(TagType.Byte, name, unchecked((byte) value))
    {
    }
        
    /// <summary>
    /// Required constructor for ISerializable implementation.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to describing this instance.</param>
    /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
    protected ByteTag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
        
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"TAG_Byte({PrettyName}): {Value}";
        
    /// <summary>
    /// Implicit conversion of this tag to a <see cref="byte"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="byte"/>.</returns>
    public static implicit operator byte(ByteTag tag) => tag.Value;
        
    /// <summary>
    /// Implicit conversion of this tag to a <see cref="sbyte"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="sbyte"/>.</returns>
    [CLSCompliant(false)]
    public static implicit operator sbyte(ByteTag tag) => unchecked((sbyte)tag.Value);

    /// <summary>
    /// Gets the <i>string</i> representation of this NBT tag (SNBT).
    /// </summary>
    /// <returns>This NBT tag in SNBT format.</returns>
    /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
    public override string Stringify() => $"{StringifyName}{Value}B";
}