using JetBrains.Annotations;

namespace SharpNBT.SNBT;

/// <summary>
/// Describes types of tokens that the SNBT lexer can emit.
/// </summary>
[PublicAPI]
public enum TokenType
{
    /// <summary>
    /// Any whitespace/newline not found within a string or identifier.
    /// </summary>
    /// <remarks>This type is not yielded during tokenization.</remarks>
    Whitespace,
        
    /// <summary>
    /// A separator between objects and array elements.
    /// </summary>
    /// <remarks>This type is not yielded during tokenization.</remarks>
    Separator,
        
    /// <summary>
    /// The beginning of new <see cref="CompoundTag"/> object.
    /// </summary>
    Compound,
        
    /// <summary>
    /// The end of a <see cref="CompoundTag"/>.
    /// </summary>
    EndCompound,
        
    /// <summary>
    /// The name of an tag.
    /// </summary>
    Identifier,
        
    /// <summary>
    /// A <see cref="StringTag"/> value, which may be contain escaped quotes.
    /// </summary>
    String,
        
    /// <summary>
    /// The beginning of a <see cref="ByteArrayTag"/>.
    /// </summary>
    ByteArray,
        
    /// <summary>
    /// The beginning of a <see cref="IntArrayTag"/>.
    /// </summary>
    IntArray,
        
    /// <summary>
    /// The beginning of a <see cref="LongArrayTag"/>.
    /// </summary>
    LongArray,
        
    /// <summary>
    /// The beginning of a <see cref="ListTag"/>.
    /// </summary>
    List,
        
    /// <summary>
    /// The end of a <see cref="ByteArrayTag"/>, <see cref="IntArrayTag"/>, <see cref="LongArrayTag"/> or <see cref="ListTag"/>.
    /// </summary>
    EndArray,
        
    /// <summary>
    /// A <see cref="ByteTag"/> value or element of a <see cref="ByteArrayTag"/> depending on context.
    /// </summary>
    Byte,
        
    /// <summary>
    /// A <see cref="BoolTag"/> value.
    /// </summary>
    Bool,
        
    /// <summary>
    /// A <see cref="ShortTag"/> value.
    /// </summary>
    Short,
        
    /// <summary>
    /// A <see cref="IntTag"/> value or element of a <see cref="IntArrayTag"/> depending on context.
    /// </summary>
    Int,
        
    /// <summary>
    /// A <see cref="LongTag"/> value or element of a <see cref="LongArrayTag"/> depending on context.
    /// </summary>
    Long,
        
    /// <summary>
    /// A <see cref="FloatTag"/> value.
    /// </summary>
    Float,
        
    /// <summary>
    /// A <see cref="DoubleTag"/> value.
    /// </summary>
    Double
}