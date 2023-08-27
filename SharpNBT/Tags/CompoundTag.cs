using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Top-level tag that acts as a container for other <b>named</b> tags. 
/// </summary>
/// <remarks>
/// This along with the <see cref="ListTag"/> class define the structure of the NBT format. Children are not order-dependent, nor is order guaranteed. The
/// closing <see cref="EndTag"/> does not require to be explicitly added, it will be added automatically during serialization. 
/// </remarks>
[PublicAPI]
public class CompoundTag : Tag, IDictionary<string, Tag>, ICollection<Tag>
{
    private readonly Dictionary<string, Tag> dict;
    
    /// <summary>
    /// Creates a new instance of the <see cref="CompoundTag"/> class.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    public CompoundTag(string? name) : base(TagType.Compound, name)
    {
        dict = new Dictionary<string, Tag>();
    }
        
    /// <summary>
    /// Creates a new instance of the <see cref="CompoundTag"/> class.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="values">A collection <see cref="Tag"/> objects that are children of this object.</param>
    public CompoundTag(string? name, IEnumerable<Tag> values) : this(name)
    {
        foreach (var value in values)
        {
            dict.Add(value.Name!, AssertName(value));
        }
    }
    
    /// <inheritdoc />
    void ICollection<KeyValuePair<string, Tag>>.Add(KeyValuePair<string, Tag> item) => dict.Add(item.Key, item.Value);

    /// <inheritdoc />
    bool ICollection<KeyValuePair<string, Tag>>.Contains(KeyValuePair<string, Tag> item) => dict.Contains(item);
    
    /// <inheritdoc />
    void ICollection<KeyValuePair<string, Tag>>.CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex)
    {
        foreach (var kvp in dict)
            array[arrayIndex++] = kvp;
    }
    
    /// <inheritdoc />
    bool ICollection<KeyValuePair<string, Tag>>.IsReadOnly => false;
    
    /// <inheritdoc />
    bool ICollection<Tag>.IsReadOnly => false;
    
    /// <inheritdoc />
    bool ICollection<KeyValuePair<string, Tag>>.Remove(KeyValuePair<string, Tag> item) => dict.Remove(item.Key);

    /// <inheritdoc cref="ICollection{T}.Clear"/>
    public void Clear() => dict.Clear();
    
    /// <inheritdoc cref="ICollection{T}.Clear"/>
    public int Count => dict.Count;
    
    /// <inheritdoc />
    IEnumerator<KeyValuePair<string, Tag>> IEnumerable<KeyValuePair<string, Tag>>.GetEnumerator() => dict.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<Tag> GetEnumerator() => dict.Values.GetEnumerator();
    
    /// <inheritdoc />
    public void CopyTo(Tag[] array, int arrayIndex)
    {
        foreach (var value in dict.Values)
            array[arrayIndex++] = value;
    }
    
    /// <inheritdoc />
    public void Add(string key, Tag value) => dict.Add(key, AssertName(value));
    
    /// <inheritdoc cref="ICollection{T}.Add"/>
    public void Add(Tag value) => dict.Add(value.Name!, AssertName(value));

    /// <inheritdoc />
    public bool ContainsKey(string key) => dict.ContainsKey(key);

    /// <inheritdoc />
    public bool Contains(Tag tag) => !string.IsNullOrEmpty(tag.Name) && dict.ContainsKey(tag.Name);

    /// <inheritdoc />
    public bool Remove(string key) => dict.Remove(key);

    /// <inheritdoc />
    public bool Remove(Tag item) => !string.IsNullOrWhiteSpace(item.Name) && dict.Remove(item.Name);

    /// <inheritdoc />
    public bool TryGetValue(string key, out Tag value) => dict.TryGetValue(key, out value!);

    /// <inheritdoc cref="TryGetValue"/>
    public bool TryGetValue<TTag>(string key, out TTag value) where TTag : Tag
    {
        if (dict.TryGetValue(key, out var tag) && tag is TTag result)
        {
            value = result;
            return true;
        }

        value = null!;
        return false;
    }
    
    /// <inheritdoc />
    public ICollection<string> Keys => dict.Keys;

    /// <inheritdoc />
    public ICollection<Tag> Values => dict.Values;
    
    /// <inheritdoc />
    public Tag this[string name]
    {
        get => dict[name];
        set => dict[name] = value;
    }

    public TTag Get<TTag>(string name) where TTag : Tag
    {
        return (TTag)dict[name];
    }
    
    /// <inheritdoc />
    protected internal override void WriteJson(Utf8JsonWriter writer, bool named = true)
    {
        if (named && Name != null)
        {
            writer.WriteStartObject(Name);
        }
        else
        {
            writer.WriteStartObject();
        }

        foreach (var child in dict.Values)
            child.WriteJson(writer, true);
        writer.WriteEndObject();
    }
    
    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.ToString?view=netcore-5.0">`Object.ToString` on docs.microsoft.com</a></footer>
    public override string ToString()
    {
        var word = Count == 1 ? Strings.WordEntry : Strings.WordEntries;
        return $"TAG_Compound({PrettyName}): [{Count} {word}]";
    }

    /// <summary>
    /// Retrieves a "pretty-printed" multiline string representing the complete tree structure of the tag.
    /// </summary>
    /// <param name="indent">The prefix that will be applied to each indent-level of nested nodes in the tree structure.</param>
    /// <returns>The pretty-printed string.</returns>
    public string PrettyPrinted(string? indent = "    ")
    {
        var buffer = new StringBuilder();
        PrettyPrinted(buffer, 0, indent ?? string.Empty);
        return buffer.ToString();
    }

    /// <summary>
    /// Searches the children of this tag, returning the first child with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the tag to search for.</param>
    /// <param name="recursive"><see langword="true"/> to recursively search children, otherwise <see langword="false"/> to only search direct descendants.</param>
    /// <returns>The first tag found with <paramref name="name"/>, otherwise <see langword="null"/> if none was found.</returns>
    public TTag? Find<TTag>(string name, bool recursive = false) where TTag : Tag
    {
        foreach (var (key, value) in dict)
        {
            if (string.CompareOrdinal(name, key) == 0 && value is TTag result)
                return result;

            if (recursive && value is CompoundTag child)
            {
                var nested = child.Find<TTag>(name, recursive);
                if (nested != null)
                    return nested;
            }
        }

        return null;
    }

    /// <inheritdoc cref="Tag.PrettyPrinted(StringBuilder,int,string)"/>
    protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
    {
        var space = new StringBuilder();
        for (var i = 0; i < level; i++)
            space.Append(indent);
            
        buffer.AppendLine(space + ToString());
        buffer.AppendLine(space + "{");
        foreach (var tag in dict.Values)
            tag.PrettyPrinted(buffer, level + 1, indent);
        buffer.AppendLine(space + "}");
    }

    /// <summary>
    /// Gets the <i>string</i> representation of this NBT tag (SNBT).
    /// </summary>
    /// <returns>This NBT tag in SNBT format.</returns>
    /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
    public override string Stringify()
    {
        var sb = new StringBuilder();
        sb.Append($"{StringifyName}:{{");

        var i = 0;
        foreach (var value in dict.Values)
        {
            if (i++ > 0)
                sb.Append(',');
            sb.Append(value);
        }
        sb.Append('}');
        return sb.ToString();
    }
        
    /// <summary>
    /// Gets the <i>string</i> representation of this NBT tag (SNBT).
    /// </summary>
    /// <param name="topLevel">Flag indicating if this is the top-level tag that should be wrapped in braces.</param>
    /// <returns>This NBT tag in SNBT format.</returns>
    /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
    public string Stringify(bool topLevel)
    {
        var str = Stringify();
        return topLevel ? $"{{{str}}}" : str;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Tag AssertName(Tag tag)
    {
        if (string.IsNullOrWhiteSpace(tag.Name))
            throw new FormatException(Strings.ChildrenMustBeNamed);
        return tag;
    }
}