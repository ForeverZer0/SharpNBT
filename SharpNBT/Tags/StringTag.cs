using System;
using System.Numerics;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag the contains a UTF-8 string.
/// </summary>
[PublicAPI][Serializable]
public class StringTag : Tag, IEquatable<StringTag>
{
    /// <summary>
    /// Gets or sets the value of the tag.
    /// </summary>
    public string Value { get; [Obsolete("String tag type will be made immutable in a future version.")] set; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="StringTag"/> class with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="value">The value to assign to this tag.</param>
    public StringTag(string? name, string? value) : base(TagType.String, name)
    {
        Value = value ?? string.Empty;
    }
        
    /// <inheritdoc />
    protected StringTag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Value = info.GetString("value") ?? string.Empty;
    }
        
    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("value", Value);
    }
    
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"TAG_String({PrettyName}): \"{Value}\"";

    /// <summary>
    /// Implicit conversion of this tag to a <see cref="string"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="string"/>.</returns>
    public static implicit operator string(StringTag tag) => tag.Value;
        
    /// <summary>
    /// Gets the <i>string</i> representation of this NBT tag (SNBT).
    /// </summary>
    /// <returns>This NBT tag in SNBT format.</returns>
    /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
    public override string Stringify() => $"{StringifyName}:\"{Value}\""; // TODO: Does this get properly escaped?

    /// <inheritdoc />
    public bool Equals(StringTag? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && string.CompareOrdinal(Value, other.Value) == 0;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((StringTag)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode(); // TODO: Add Value once immutable
    
    /// <summary>
    /// Compares two values to determine equality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator ==(StringTag? left, StringTag? right) => Equals(left, right);

    /// <summary>
    /// Compares two values to determine inequality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator !=(StringTag? left, StringTag? right) => !Equals(left, right);
}