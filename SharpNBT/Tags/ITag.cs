using System.IO;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Represents an NBT tag type.
/// </summary>
[PublicAPI]
public interface ITag
{
    /// <summary>
    /// Gets a constant that describes the NBT tag type.
    /// </summary>
    public static abstract TagType Type { get; }
    
    /// <summary>
    /// Gets the name of the NBT tag. 
    /// </summary>
    /// <remarks>
    /// Tags typically require a name, with the exception of the children of a <see cref="ListTag"/> and
    /// the top-level <see cref="CompoundTag"/> for the document.
    /// </remarks>
    string? Name { get; }

    /// <summary>
    /// Outputs the NBT tag in a human-readable format to the standard output stream.
    /// </summary>
    void PrettyPrint();
    
    /// <summary>
    /// Outputs the NBT tag in a human-readable format to the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> to write the output to.</param>
    /// <exception cref="IOException">The <paramref name="stream"/> is not opened for writing.</exception>
    void PrettyPrint(Stream stream);
}

// TODO

[PublicAPI]
public interface IValueTag<T> : ITag
{
    T Value { get; }
}