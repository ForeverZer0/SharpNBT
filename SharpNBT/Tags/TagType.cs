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
    /// <remarks>Some implementation may also use as the child type for an empty list.</remarks>
    End = 0x00,

    /// <summary>
    /// A single signed byte,
    /// </summary>
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