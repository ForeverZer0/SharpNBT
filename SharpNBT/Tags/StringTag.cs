using System;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag containing a string value.
/// </summary>
[PublicAPI]
public class StringTag : Tag, IValueTag<string>, IEquatable<StringTag>
{
    private string? stringValue;
    private byte[]? utf8Bytes;
    
    /// <inheritdoc />
    static TagType ITag.Type => TagType.String;

    /// <summary>
    /// Gets the value of the tag.
    /// </summary>
    public string Value
    {
        get
        {
            if (stringValue != null)
                return stringValue;
            
            stringValue = utf8Bytes is null ? string.Empty : Encoding.UTF8.GetString(utf8Bytes);
            return stringValue;
        }
    }

    /// <summary>
    /// Gets the UTF-8 bytes for the tag's <see cref="Value"/>.
    /// </summary>
    public byte[] Bytes
    {
        get
        {
            if (utf8Bytes != null)
                return utf8Bytes;

            utf8Bytes = string.IsNullOrEmpty(stringValue) ? Array.Empty<byte>() :  Encoding.UTF8.GetBytes(Value);
            return utf8Bytes;
        }
    }

    /// <summary>
    /// Gets the number of UTF-8 bytes the value contains.
    /// </summary>
    public int ByteCount => utf8Bytes?.Length ?? Encoding.UTF8.GetByteCount(Value);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="StringTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public StringTag(string? name, string? value) : base(name)
    {
        stringValue = value ?? string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="StringTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public StringTag(string? name, ReadOnlySpan<byte> value) : base(name)
    {
        utf8Bytes = value.ToArray();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="StringTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public StringTag(string? name, byte[] value) : base(name)
    {
        utf8Bytes = value;
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_String({PrettyName}): \"{Value}\"";
    
    /// <summary>
    /// Implicit conversion of a an <see cref="StringTag"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="tag">The <see cref="StringTag"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="string"/>.</returns>
    public static implicit operator string(StringTag tag) => tag.Value;

    /// <inheritdoc />
    public bool Equals(StringTag? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.CompareOrdinal(Value, other.Value) == 0;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((StringTag)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Value);

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