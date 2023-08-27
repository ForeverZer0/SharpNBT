using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Represents a collection of a tags.
/// </summary>
/// <remarks>
/// All child tags <b>must</b> be have the same <see cref="Tag.Type"/> value, and their <see cref="Tag.Name"/> value will be omitted during serialization.
/// </remarks>
[PublicAPI][Serializable]
public class ListTag : Tag, IList<Tag>
{
    /// <summary>
    /// Gets the NBT type of this tag's children.
    /// </summary>
    public TagType ChildType { get; private set; }

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
            list.Add(AssertType(item));
    }
        
    /// <summary>
    /// Required constructor for ISerializable implementation.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to describing this instance.</param>
    /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
    protected ListTag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ChildType = (TagType)info.GetByte("child_type");
        var count = info.GetInt32("count");
        list = new List<Tag>(count);
        if (info.GetValue("values", typeof(Tag[])) is Tag[] ary)
            AddRange(ary);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("child_type", (byte) ChildType);
        info.AddValue("count", list.Count);
        info.AddValue("values", list.ToArray());
    }

    /// <inheritdoc />
    public IEnumerator<Tag> GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    public void Add(Tag item) => list.Add(AssertType(item));

    public void AddRange(IEnumerable<Tag> items)
    {
        foreach (var item in items)
            list.Add(AssertType(item));
    }

    /// <inheritdoc />
    public void Clear() => list.Clear();

    /// <inheritdoc />
    public bool Contains(Tag item) => list.Contains(item);

    /// <inheritdoc />
    public void CopyTo(Tag[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(Tag item) => list.Remove(item);

    /// <inheritdoc />
    public int Count => list.Count;

    /// <inheritdoc />
    bool ICollection<Tag>.IsReadOnly => false;

    /// <inheritdoc />
    public int IndexOf(Tag item) => list.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, Tag item) => list.Insert(index, AssertType(item));

    /// <inheritdoc />
    public void RemoveAt(int index) => list.RemoveAt(index);

    /// <inheritdoc />
    public Tag this[int index]
    {
        get => list[index];
        set => list[index] = AssertType(value);
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
    private Tag AssertType(Tag tag)
    {
        if (tag.Type != ChildType)
            throw new ArrayTypeMismatchException(Strings.ChildWrongType);
        return tag;
    }
    
    private readonly List<Tag> list;
    

}