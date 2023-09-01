using System;
using System.Text.Json;
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
public class ByteTag : NumericTag<byte>
{
    /// <summary>
    /// Gets a flag indicating if this <see cref="ByteTag"/> was assigned a <see cref="bool"/> value.
    /// </summary>
    public bool IsBool { get; private set; }
    
    /// <inheritdoc cref="Tag{T}.Value"/>
    public new byte Value
    {
        get => base.Value;
        set
        {
            base.Value = value;
            IsBool = false;
        }
    }
    
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
        set
        {
            Value = unchecked((byte)value);
            IsBool = false;
        }
    }
    
    /// <summary>
    /// Gets or sets the value of this tag as a boolean value.
    /// </summary>
    public bool Bool
    {
        get => Value != 0;
        set
        {
            Value = value ? (byte)1 : (byte)0;
            IsBool = true;
        }
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
    /// <remarks>The use of <see cref="int"/> is for convenience only.</remarks>
    public ByteTag(string? name, int value) : base(TagType.Byte, name, unchecked((byte) (value & 0xFF)))
    {
    }
    
    /// <inheritdoc cref="ByteTag(string,byte)"/>
    public ByteTag(string? name, bool value) : base(TagType.Byte, name, value ? (byte) 1 : (byte) 0)
    {
    }
        
    /// <inheritdoc cref="ByteTag(string,byte)"/>
    [CLSCompliant(false)]
    public ByteTag(string? name, sbyte value) : base(TagType.Byte, name, unchecked((byte) value))
    {
    }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        object obj = IsBool ? Bool : Value;
        return $"TAG_Byte({PrettyName}): {obj}";
    }
    
    /// <inheritdoc />
    protected internal override void WriteJson(Utf8JsonWriter writer, bool named = true)
    {
        if (named && Name != null)
        {
            if (IsBool)
                writer.WriteBoolean(Name, Bool);
            else
                writer.WriteNumber(Name, Value);
        }
        else
        {
            writer.WriteNumberValue(Value);
        }
    }
        
    /// <summary>
    /// Implicit conversion of this tag to a <see cref="byte"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="byte"/>.</returns>
    public static implicit operator byte(ByteTag tag) => tag.Value;
    
    /// <summary>
    /// Implicit conversion of this tag to a <see cref="bool"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="byte"/>.</returns>
    public static implicit operator bool(ByteTag tag) => tag.Bool;
        
    /// <summary>
    /// Implicit conversion of this tag to a <see cref="sbyte"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="sbyte"/>.</returns>
    [CLSCompliant(false)]
    public static implicit operator sbyte(ByteTag tag) => unchecked((sbyte)tag.Value);

    /// <inheritdoc />
    public override string Stringify(bool named = true) => named ? $"{StringifyName}:{Value}B" : $"{Value}B"; 
}