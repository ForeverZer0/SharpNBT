using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpNBT;

/// <summary>
/// An NBT tag containing a collection of <b>named</b> tags.
/// </summary>
/// <remarks>Order is not guaranteed.</remarks>
public class CompoundTag : Tag, IDictionary<string, Tag>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Compound;
    
    private readonly Dictionary<string, Tag> dict;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    public CompoundTag(string? name) : base(name)
    {
        dict = new Dictionary<string, Tag>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="values">A collection of values to populate the tag with.</param>
    public CompoundTag(string? name, IEnumerable<Tag> values) : base(name)
    {
        dict = new Dictionary<string, Tag>();
        foreach (var value in values)
        {
            dict.Add(value.Name!, AssertName(value));
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the NBT tag, or <see langword="null"/> when the tag has no name.</param>
    /// <param name="values">A collection of values to populate the tag with.</param>
    public CompoundTag(string? name, IReadOnlyDictionary<string, Tag> values) : base(name)
    {
        dict = new Dictionary<string, Tag>();
        foreach (var (childName, value) in values)
        {
            dict.Add(childName, AssertName(value));
        }
    }
    
    private Tag AssertName(Tag tag)
    {
        if (string.IsNullOrWhiteSpace(tag.Name))
            throw new ArgumentException("Compound tags require all children to have a valid name.");
        return tag;
    }
    
    /// <inheritdoc cref="ICollection{T}.Add" />
    public void Add(Tag item) => dict.Add(item.Name!, AssertName(item));

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, Tag>> GetEnumerator() => dict.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)dict).GetEnumerator();

    /// <inheritdoc />
    public void Add(KeyValuePair<string, Tag> item) => dict.Add(item.Key, AssertName(item.Value));

    /// <inheritdoc />
    public void Clear() => dict.Clear();

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, Tag> item) => dict.Contains(item);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex)
    {
        foreach (var kvp in dict)
        {
            array[arrayIndex++] = kvp;
        }
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<string, Tag> item) => dict.Remove(item.Key);

    /// <inheritdoc />
    public int Count => dict.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(string key, Tag value) => dict.Add(key, AssertName(value));

    /// <inheritdoc />
    public bool ContainsKey(string key) => dict.ContainsKey(key);

    /// <inheritdoc />
    public bool Remove(string key) => dict.Remove(key);

    /// <inheritdoc />
    public bool TryGetValue(string key, out Tag value) => dict.TryGetValue(key, out value!);

    /// <inheritdoc />
    public Tag this[string key]
    {
        get => dict[key];
        set => dict[key] = AssertName(value);
    }

    /// <inheritdoc />
    public ICollection<string> Keys => dict.Keys;

    /// <inheritdoc />
    public ICollection<Tag> Values => dict.Values;
    
    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_Compound({PrettyName}): {Count} {word}";
    }

    /// <inheritdoc />
    protected internal override void PrettyPrint(IndentedTextWriter writer)
    {
        base.PrettyPrint(writer);
        writer.WriteLine('{');
        writer.Indent++;
        foreach (var tag in dict.Values)
            tag.PrettyPrint(writer);
        writer.Indent--;
        writer.WriteLine('}');
    }
}