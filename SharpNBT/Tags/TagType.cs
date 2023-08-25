using System;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Strongly-typed numerical constants that are prefixed to tags to denote their type.
/// </summary>
[PublicAPI][Serializable]
public enum TagType : byte
{
    /// <summary>
    /// Signifies the end of a <see cref="CompoundTag"/>.
    /// </summary>
    End = 0x00,

    /// <summary>
    /// A single signed byte,
    /// </summary>
    /// <remarks>
    /// This uses an unsigned byte to represent this type, contrary to the NBT specification using a signed value
    /// due to the language differences of Java. This is a deliberate design decision, for the following reasons:
    /// <list type="bullet">
    /// <item>
    /// A <see cref="sbyte"/> is not a CLR type. Using it would prevent other CLR languages from consuming this library.
    /// </item>
    /// <item>
    /// In practical terms, they are used for the same purpose. A byte in NBT is rarely, if every used as a "counting"
    /// integer, but similar to a .NET <see cref="byte"/>, is used for raw buffers of data.
    /// </item>
    /// <item>
    /// It makes little difference, the type is available as it native value in languages that support it, and the
    /// in-memory representation is exactly the same.</item>
    /// </list>
    /// </remarks>
    Byte = 0x01,

    /// <summary>
    /// A single signed 16-bit integer.
    /// </summary>
    Short = 0x02,

    /// <summary>
    /// A single signed 32-bit integer.
    /// </summary>
    Int = 0x03,

    /// <summary>
    /// A single signed 64-bit integer.
    /// </summary>
    Long = 0x04,

    /// <summary>
    /// A single IEEE-754 single-precision floating point number.
    /// </summary>
    Float = 0x05,

    /// <summary>
    /// A single IEEE-754 double-precision floating point number.
    /// </summary>
    Double = 0x06,

    /// <summary>
    /// A length-prefixed array of bytes.
    /// </summary>
    ByteArray = 0x07,

    /// <summary>
    /// A length-prefixed UTF-8 string.
    /// </summary>
    String = 0x08,

    /// <summary>
    /// A list of nameless tags, all of the same type.
    /// </summary>
    List = 0x09,

    /// <summary>
    /// A set of named tags.
    /// </summary>
    Compound = 0x0a,

    /// <summary>
    /// A length-prefixed array of signed 32-bit integers.
    /// </summary>
    IntArray = 0x0b,

    /// <summary>
    /// A length-prefixed array of signed 64-bit integers.
    /// </summary>
    LongArray = 0x0c
}