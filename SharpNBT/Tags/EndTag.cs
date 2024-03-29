using System.Text;
using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Represents the end of <see cref="CompoundTag"/>.
/// </summary>
[PublicAPI]
public sealed class EndTag : Tag
{
    /// <summary>
    /// Creates a new instance of the <see cref="EndTag"/> class.
    /// </summary>
    public EndTag() : base(TagType.End, null)
    {
    }
     
    /// <inheritdoc />
    protected internal override void WriteJson(Utf8JsonWriter writer, bool named = true)
    {
        // Do nothing
    }
    
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"TAG_End";

    /// <inheritdoc />
    protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
    {
        // Do nothing
    }
    
    /// <inheritdoc />
    public override string Stringify(bool named = true) => string.Empty;
}