using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Represents a collection of a tags.
/// </summary>
/// <remarks>
/// All child tags <b>must</b> be have the same <see cref="Tag.Type"/> value, and their <see cref="Tag.Name"/> value will be omitted during serialization.
/// </remarks>
[PublicAPI]
public class ListTag : Tag, IList<Tag>
{
    /// <summary>
    /// Gets the NBT type of this tag's children.
    /// </summary>
    public TagType ChildType { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="childType">A constant describing the NBT type for children in this tag.</param>
    public ListTag(string? name, TagType childType) : base(TagType.List, name)
    {
        ChildType = childType;
        list = new List<Tag>();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="childType">A constant describing the NBT type for children in this tag.</param>
    /// <param name="capacity">The initial capacity of the list.</param>
    public ListTag(string? name, TagType childType, int capacity) : base(TagType.List, name)
    {
        ChildType = childType;
        list = new List<Tag>(capacity);
    }
        
    /// <summary>
    /// Creates a new instance of the <see cref="ListTag"/> with the specified <paramref name="children"/>.
    /// </summary>
    /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
    /// <param name="childType">A constant describing the NBT type for children in this tag.</param>
    /// <param name="children">A collection of values to include in this tag.</param>
    public ListTag(string? name, TagType childType, IEnumerable<Tag> children) : this(name, childType)
    {
        foreach (var item in children)
            list.Add(ValidateChild(item));
    }

    /// <inheritdoc />
    public IEnumerator<Tag> GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    public void Add(Tag item) => list.Add(ValidateChild(item));

    public void AddRange(IEnumerable<Tag> items)
    {
        foreach (var item in items)
            list.Add(ValidateChild(item));
    }

    /// <inheritdoc />
    public void Clear() => list.Clear();

    /// <inheritdoc />
    public bool Contains(Tag item) => list.Contains(item);

    /// <inheritdoc />
    public void CopyTo(Tag[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(Tag item)
    {
        if (list.Remove(item))
        {
            item.Parent = null;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public int Count => list.Count;

    /// <inheritdoc />
    bool ICollection<Tag>.IsReadOnly => false;

    /// <inheritdoc />
    public int IndexOf(Tag item) => list.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, Tag item) => list.Insert(index, ValidateChild(item));

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        var item = list[index];
        item.Parent = null;
        list.RemoveAt(index);
    }

    /// <inheritdoc />
    public Tag this[int index]
    {
        get => list[index];
        set => list[index] = ValidateChild(value);
    }
    
    /// <inheritdoc />
    protected internal override void WriteJson(Utf8JsonWriter writer, bool named = true)
    {
        if (named && Name != null)
        {
            writer.WriteStartArray(Name);
        }
        else
        {
            writer.WriteStartArray();
        }
        
        for (var i = 0; i < Count; i++)
            list[i].WriteJson(writer, false);
        writer.WriteEndArray();
    }
    
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        var word = Count == 1 ? Strings.WordEntry : Strings.WordEntries;
        return $"TAG_List({PrettyName}): [{Count} {word}]";
    }
    
    /// <inheritdoc cref="Tag.PrettyPrinted(StringBuilder,int,string)"/>
    protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
    {
        var space = new StringBuilder();
        for (var i = 0; i < level; i++)
            space.Append(indent);
            
        buffer.AppendLine(space + ToString());
        buffer.AppendLine(space + "{");
        foreach (var tag in this)
            tag.PrettyPrinted(buffer, level + 1, indent);
        buffer.AppendLine(space + "}");
    }

    /// <summary>
    /// Retrieves a "pretty-printed" multiline string representing the complete tree structure of the tag.
    /// </summary>
    /// <param name="indent">The prefix that will be applied to each indent-level of nested nodes in the tree structure.</param>
    /// <returns>The pretty-printed string.</returns>
    public string PrettyPrinted(string indent = "    ")
    {
        var buffer = new StringBuilder();
        PrettyPrinted(buffer, 0, indent);
        return buffer.ToString();
    }

    /// <summary>
    /// Gets the <i>string</i> representation of this NBT tag (SNBT).
    /// </summary>
    /// <returns>This NBT tag in SNBT format.</returns>
    /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
    public override string Stringify()
    {
        var strings = new string[Count];
        for (var i = 0; i < strings.Length; i++)
            strings[i] = this[i].Stringify();
            
        // TODO: Use StringBuilder
        
        return $"{StringifyName}:[{string.Join(',', strings)}]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Tag ValidateChild(Tag tag)
    {
        if (tag.Type != ChildType)
            throw new ArrayTypeMismatchException(Strings.ChildWrongType);
        tag.Parent = this;
        return tag;
    }
    
    private readonly List<Tag> list;
}