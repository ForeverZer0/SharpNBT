using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// An NBT tag that containing a 64-bit IEEE-754 double-precision floating point number.
/// </summary>
[PublicAPI]
public class DoubleTag : NumericTag<double>, IValueTag<double>
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Double;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="value">The value of the tag.</param>
    public DoubleTag(string? name, double value) : base(name, value)
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Double({PrettyName}): {Value:0.0}";
}