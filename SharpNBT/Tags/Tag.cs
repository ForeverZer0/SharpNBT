using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("SharpNBT.Tests")]

namespace SharpNBT;

/// <summary>
/// Abstract base class that all NBT tags inherit from.
/// </summary>
[PublicAPI]
public abstract class Tag : IEquatable<Tag>, ICloneable
{
    /// <summary>
    /// Text applied in a pretty-print sting when a tag has no defined <see cref="Name"/> value.
    /// </summary>
    protected const string NoName = "None";
        
    /// <summary>
    /// Gets a constant describing the NBT type this object represents.
    /// </summary>
    public TagType Type { get; }
        
    /// <summary>
    /// Gets the parent <see cref="Tag"/> this object is a child of.
    /// </summary>
    [Obsolete("Parent property may be removed in a future version.")]
    public Tag? Parent { get; internal set; }
        
    /// <summary>
    /// Gets the name assigned to this <see cref="Tag"/>.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Initialized a new instance of the <see cref="Tag"/> class.
    /// </summary>
    /// <param name="type">A constant describing the NBT type for this tag.</param>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    protected Tag(TagType type, string? name)
    {
        Type = type;
        Name = name;
    }
    
    /// <summary>
    /// Writes this tag as a formatted string to the given <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">A <see cref="StringBuilder"/> instance to write to.</param>
    /// <param name="level">The current indent depth to write at.</param>
    /// <param name="indent">The string to use for indents.</param>
    protected internal virtual void PrettyPrinted(StringBuilder buffer, int level, string indent)
    {
        for (var i = 0; i < level; i++)
            buffer.Append(indent);
        buffer.AppendLine(ToString());
    }

    protected internal virtual void WarppedPrinted(StringBuilder buffer, int level, string indent)
    {
        for (var i = 0; i < level; i++)
            buffer.Append(indent);
        buffer.AppendLine(ToWarppedString());
    }
    public virtual string ToWarppedString() => "";
    protected internal string WarpedName => Name is null ? "" : $"{Name}";

    /// <summary>
    /// Gets the name of the object as a human-readable quoted string, or a default name to indicate it has no name when applicable.
    /// </summary>
    protected internal string PrettyName => Name is null ? "None" : $"\"{Name}\"";
    
    /// <summary>
    /// Uses the provided <paramref name="writer"/> to write the NBT tag in JSON format.
    /// </summary>
    /// <param name="writer">A JSON writer instance.</param>
    /// <param name="named">
    /// Flag indicating if this object's name should be written as a property name, or <see langword="false"/> when it
    /// is a child of <see cref="ListTag"/>, in which case it should be written as a JSON array element.
    /// </param>
    protected internal abstract void WriteJson(Utf8JsonWriter writer, bool named = true);
    
    /// <summary>
    /// Writes the tag to the specified <paramref name="stream"/> in JSON format.
    /// </summary>
    /// <param name="stream">The stream instance to write to.</param>
    /// <param name="options">Options that will be passed to the JSON writer.</param>
    /// <exception cref="IOException">The stream is no opened for writing.</exception>
    public void WriteJson(Stream stream, JsonWriterOptions? options = null)
    {
        using var json = new Utf8JsonWriter(stream, options ?? new JsonWriterOptions());
        
        if (string.IsNullOrEmpty(Name))
        {
            json.WriteStartArray();
            WriteJson(json, false);
            json.WriteEndArray();
        }
        else
        {
            json.WriteStartObject();
            WriteJson(json, true);
            json.WriteEndObject();
        }
        
        json.Flush();
    }
    
    /// <summary>
    /// Asynchronously writes the tag to the specified <paramref name="stream"/> in JSON format.
    /// </summary>
    /// <param name="stream">The stream instance to write to.</param>
    /// <param name="options">Options that will be passed to the JSON writer.</param>
    /// <exception cref="IOException">The stream is no opened for writing.</exception>
    public async Task WriteJsonAsync(Stream stream, JsonWriterOptions? options = null)
    {
        await using var json = new Utf8JsonWriter(stream, options ?? new JsonWriterOptions());
        
        if (string.IsNullOrEmpty(Name))
        {
            json.WriteStartArray();
            WriteJson(json, false);
            json.WriteEndArray();
        }
        else
        {
            json.WriteStartObject();
            WriteJson(json, true);
            json.WriteEndObject();
        }
        
        await json.FlushAsync();
    }

    /// <summary>
    /// Converts the NBT to an equivalent JSON representation, and returns it as a string.
    /// </summary>
    /// <param name="options">Options that will be passed to the JSON writer.</param>
    /// <returns>The JSON-encoded string representing describing the tag.</returns>
    public string ToJson(JsonWriterOptions? options = null)
    {
        using var stream = new MemoryStream();
        WriteJson(stream, options);
        stream.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
    
    /// <summary>
    /// Gets a representation of this <see cref="Tag"/> as a JSON string.
    /// </summary>
    /// <param name="pretty">Flag indicating if formatting should be applied to make the string human-readable.</param>
    /// <param name="indent">Ignored</param>
    /// <returns>A JSON string describing this object.</returns>
    [Obsolete("Use WriteJson and ToJson instead.")]
    public string ToJsonString(bool pretty = false, string indent = "    ")
    {
        var options = new JsonWriterOptions { Indented = pretty };
        return ToJson(options);
    }

    /// <inheritdoc />
    public bool Equals(Tag? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && string.CompareOrdinal(Name, other.Name) == 0;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Tag)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine((int)Type, Name);
    
    /// <inheritdoc />
    public object Clone()
    {
        // Serialize then deserialize to make a deep-copy
        using var stream = new MemoryStream();
            
        // Use native endian 
        var opts = BitConverter.IsLittleEndian ? FormatOptions.LittleEndian : FormatOptions.BigEndian;
        using var writer = new TagWriter(stream, opts, true);
        using var reader = new TagReader(stream, opts, true);
                
        writer.WriteTag(this);
        stream.Seek(0, SeekOrigin.Begin);
            
        return reader.ReadTag(!string.IsNullOrWhiteSpace(Name));
    }

    
    /// <summary>
    /// Tests for equality of this object with another <see cref="Tag"/> instance.
    /// </summary>
    /// <param name="left">First value to compare.</param>
    /// <param name="right">Second value to compare.</param>
    /// <returns>Result of comparison.</returns>
    public static bool operator ==(Tag? left, Tag? right) => Equals(left, right);

    /// <summary>
    /// Tests for inequality of this object with another <see cref="Tag"/> instance.
    /// </summary>
    /// <param name="left">First value to compare.</param>
    /// <param name="right">Second value to compare.</param>
    /// <returns>Result of comparison.</returns>
    public static bool operator !=(Tag? left, Tag? right) => !Equals(left, right);
        
    /// <summary>
    /// Gets the <i>string</i> representation of this NBT tag (SNBT).
    /// </summary>
    /// <param name="named">Flag indicating if the name of the tag should be written.</param>
    /// <returns>This NBT tag in SNBT format.</returns>
    /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
    public abstract string Stringify(bool named = true);

    /// <summary>
    /// Gets the name in a formatted properly for SNBT.
    /// </summary>
    protected internal string StringifyName
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
                return string.Empty;
            return Name.All(c => c.IsValidUnquoted()) ? Name : $"\"{Name}\"";
        }
    }
}