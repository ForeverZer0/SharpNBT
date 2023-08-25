namespace SharpNBT;

/// <summary>
/// An NBT tag used to denote the end of a <see cref="CompoundTag"/>.
/// </summary>
/// <remarks>
/// This class is not actually included used in the deserialized content, but merely as a marker when reading/writing
/// to/from a stream.
/// </remarks>
public class EndTag : Tag, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.End;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EndTag"/> class.
    /// </summary>
    public EndTag() : base(null)
    {
    }

    /// <inheritdoc />
    public override string ToString() => "TAG_End";
}