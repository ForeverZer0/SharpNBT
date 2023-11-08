using System;
using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag that contains a single 8-bit integer value.
/// </summary>
/// <remarks>
/// This tag type does not exist in the NBT specification, and is included for convenience to differentiate it from the <see cref="ByteTag"/> that it is
/// actually serialized as.
/// </remarks>
[PublicAPI]
[Obsolete("Use the IsBool and Bool properties of ByteTag. This class will be removed in a future version.")]
public class BoolTag : Tag
{
    public bool Value { get; set; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="SharpNBT.ByteTag"/> class with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="value">The value to assign to this tag.</param>
    public BoolTag(string? name, bool value) : base(TagType.Byte, name)
    {
        Value = value;
    }
 
    /// <inheritdoc />
    protected internal override void WriteJson(Utf8JsonWriter writer, bool named = true)
    {
        if (named && Name != null)
        {
            writer.WriteBoolean(Name, Value);
        }
        else
        {
            writer.WriteBooleanValue(Value);
        }
    }
    
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"TAG_Byte({PrettyName}): {(Value ? "true" : "false")}";
    public override string ToWarppedString() => $"T{PrettyName}: {(Value ? "true" : "false")}";

    /// <summary>
    /// Implicit conversion of this tag to a <see cref="byte"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="byte"/>.</returns>
    public static implicit operator bool(BoolTag tag) => tag.Value;

    /// <inheritdoc />
    public override string Stringify(bool named = true)
    {
        var value = Value ? "true" : "false";
        return named ? $"{StringifyName}:{value}" : value;
    }
        
}