using System;
using System.CodeDom.Compiler;
using System.IO;
using JetBrains.Annotations;

[assembly: CLSCompliant(true)]

namespace SharpNBT;

/// <summary>
/// Abstract base class for NBT tag types.
/// </summary>
[PublicAPI]
public abstract class Tag : IEquatable<Tag>
{
    /// <summary>
    /// The default name when the NBT tag has no name. This is for display/debugging purposes only.
    /// </summary>
    protected const string NoName = "None";
    
    /// <inheritdoc />
    public string? Name { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    protected Tag(string? name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? null : name;
    }

    /// <inheritdoc />
    public void PrettyPrint()
    {
        using var writer = new IndentedTextWriter(Console.Out);
        PrettyPrint(writer);
    }

    /// <inheritdoc />
    public void PrettyPrint(Stream stream)
    {
        using var streamWriter = new StreamWriter(stream, null, -1, true);
        using var writer = new IndentedTextWriter(streamWriter);
        PrettyPrint(writer);
    }

    /// <summary>
    /// Outputs the NBT tag in a human-readable format to the specified <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">A text-writer that will be used for writing the output.</param>
    protected internal virtual void PrettyPrint(IndentedTextWriter writer) => writer.WriteLine(ToString());

    /// <summary>
    /// Gets a string suitable for labeling the NBT tag, enclosing in quotation marks and supplying a default name
    /// where necessary.
    /// </summary>
    protected string PrettyName => string.IsNullOrWhiteSpace(Name) ? NoName : $"\"{Name}\"";
    
    /// <inheritdoc />
    public bool Equals(Tag? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Tag)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => (Name != null ? Name.GetHashCode() : 0);

    /// <summary>
    /// Compares two values to determine equality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator ==(Tag? left, Tag? right) => Equals(left, right);

    /// <summary>
    /// Compares two values to determine inequality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator !=(Tag? left, Tag? right) => !Equals(left, right);
}