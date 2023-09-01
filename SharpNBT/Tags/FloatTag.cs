using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// A tag that contains a single IEEE-754 single-precision floating point number.
/// </summary>
[PublicAPI]
public class FloatTag : NumericTag<float>
{
    /// <summary>
    /// Creates a new instance of the <see cref="FloatTag"/> class with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="value">The value to assign to this tag.</param>
    public FloatTag(string? name, float value) : base(TagType.Float, name, value)
    {
    }
    
    /// <inheritdoc />
    protected internal override void WriteJson(Utf8JsonWriter writer, bool named = true)
    {
        if (named && Name != null)
        {
            writer.WriteNumber(Name, Value);
        }
        else
        {
            writer.WriteNumberValue(Value);
        }
    }
        
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"TAG_Float({PrettyName}): {Value:0.0}";
        
    /// <summary>
    /// Implicit conversion of this tag to a <see cref="float"/>.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag represented as a <see cref="float"/>.</returns>
    public static implicit operator float(FloatTag tag) => tag.Value;
        
    /// <inheritdoc />
    public override string Stringify(bool named = true) => named ? $"{StringifyName}:{Value}F" : $"{Value}F";
}