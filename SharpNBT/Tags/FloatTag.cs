using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag that containing a 32-bit IEEE-754 single-precision floating point number.
/// </summary>
[PublicAPI]
public class FloatTag : NumericTag<float>, IValueTag<float>
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Float;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="FloatTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public FloatTag(string? name, float value) : base(name, value)
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Float({PrettyName}): {Value:0.0}";
}