using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Represents a collection of NBT tags.
/// </summary>
/// <typeparam name="TTag">A <see cref="Tag"/> type that implements <see cref="ITag"/>.</typeparam>
public interface IListTag<TTag> : ITag, IList<TTag> where TTag : ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.List;
    
    /// <summary>
    /// Gets a constant describing the type of child NBT tag in the collection.
    /// </summary>
    TagType ChildType { get; }
}

/// <summary>
/// Represents a collection of NBT tags.
/// </summary>
/// <typeparam name="TTag">A <see cref="Tag"/> type that implements <see cref="ITag"/>.</typeparam>
/// <remarks>
/// This is the recommended collection type to use over the <see cref="ListTag"/> type, as type checking for the child
/// tags is performed at compile-time instead of runtime.
/// </remarks>
[PublicAPI]
public class ListTag<TTag> : Tag, IListTag<TTag> where TTag : ITag
{
    private readonly List<TTag> list;

    /// <inheritdoc />
    public TagType ChildType => TTag.Type;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    /// <param name="values">A collection of values to add to the list.</param>
    public ListTag(string? name, IEnumerable<TTag> values) : base(name)
    {
#if DEBUG
        list = new List<TTag>();  
        foreach (var value in values)
        {
            WarnIfName(value);
            list.Add(value);
        }
#else
          list = new List<TTag>(values);    
#endif
    }

    [Conditional("DEBUG")]
    private void WarnIfName(TTag tag)
    {
        if (tag.Name != null)
            Debug.WriteLine($"Tag with name \"{tag.Name}\" added to list. Name will be ignored during serialization.");     
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    public ListTag(string? name) : base(name)
    {
        list = new List<TTag>();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    /// <param name="capacity">The initial capacity of the list.</param>
    public ListTag(string? name, int capacity) : base(name)
    {
        list = new List<TTag>(capacity);
    }
    
    /// <inheritdoc />
    public IEnumerator<TTag> GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

    /// <inheritdoc />
    public void Add(TTag item)
    {
        WarnIfName(item);
        list.Add(item);
    }

    /// <inheritdoc />
    public void Clear() => list.Clear();

    /// <inheritdoc />
    public bool Contains(TTag item) => list.Contains(item);

    /// <inheritdoc />
    public void CopyTo(TTag[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(TTag item) => list.Remove(item);

    /// <inheritdoc />
    public int Count => list.Count;

    /// <inheritdoc />
    bool ICollection<TTag>.IsReadOnly => false;

    /// <inheritdoc />
    public int IndexOf(TTag item) => list.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, TTag item)
    {
        WarnIfName(item);
        list.Insert(index, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index) => list.RemoveAt(index);

    /// <inheritdoc />
    public TTag this[int index]
    {
        get => list[index];
        set
        {
            WarnIfName(value);
            list[index] = value;
        }
    }
}

/// <summary>
/// Represents a collection of NBT tags.
/// </summary>
/// <remarks>
/// All children of a <see cref="ListTag"/> must be of the same type, which is enforced by the library. Initially a new
/// list uses a type of <see cref="TagType.End"/>, which will be updated upon the first insertion. Any additional
/// insertions must be of the same type, else an exception will be thrown. Conversely, if a list is cleared or all of
/// its items removed, it will revert back to <see cref="TagType.End"/>, and is free to insert a different type.
/// <para/>
/// When building your own list, it is recommended to use <see cref="ListTag{T}"/>, as it does type checking at
/// compile-time. This implementation requires runtime-checks during insertion of new items to ensure a collection that
/// is valid for the NBT format.
/// </remarks>
[PublicAPI]
public class ListTag : Tag, IListTag<Tag>
{
    private TagType? childType;
    private readonly List<Tag> list;

    /// <inheritdoc />
    /// <remarks>Returns <see cref="TagType.End"/> when no type is set.</remarks>
    public TagType ChildType => childType ?? TagType.End;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    /// <param name="values">A collection of values to add to the list.</param>
    public ListTag(string? name, IEnumerable<Tag> values) : this(name)
    {
        foreach (var tag in values)
            list.Add(AssertType(tag));
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    public ListTag(string? name) : base(name)
    {
        list = new List<Tag>();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    /// <param name="capacity">The initial capacity of the list.</param>
    public ListTag(string? name, int capacity) : base(name)
    {
        list = new List<Tag>(capacity);
    }

    private TTag AssertType<TTag>(TTag tag) where TTag : ITag
    {
        if (childType is null)
        {
            childType = TTag.Type;
        }
        else if (TTag.Type != childType)
        {
            throw new ArgumentException($"Invalid child type. Expected type of {childType}, got {TTag.Type}.");
        }
        
#if DEBUG
        if (tag.Name != null)
            Debug.WriteLine($"Tag with name \"{tag.Name}\" added to list. Name will be ignored during serialization.");  
#endif
        
        return tag;
    }

    /// <inheritdoc />
    public IEnumerator<Tag> GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

    /// <inheritdoc cref="ICollection{T}.Add" />
    public void Add(Tag item) => list.Add(AssertType(item));

    /// <inheritdoc />
    public void Clear()
    {
        list.Clear();
        childType = null;
    }

    /// <inheritdoc />
    public bool Contains(Tag item) => list.Contains(item);

    /// <inheritdoc />
    public void CopyTo(Tag[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(Tag item)
    {
        var result = list.Remove(item);
        if (Count == 0)
            childType = null;
        return result;
    }

    /// <inheritdoc />
    public int Count => list.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public int IndexOf(Tag item) => list.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, Tag item) => list.Insert(index, AssertType(item));

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
        if (Count == 0)
            childType = null;
    }

    /// <inheritdoc />
    public Tag this[int index]
    {
        get => list[index];
        set => list[index] = AssertType(value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_List({PrettyName}): {Count} {word}";
    }

    /// <inheritdoc />
    protected internal override void PrettyPrint(IndentedTextWriter writer)
    {
        base.PrettyPrint(writer);
        writer.WriteLine('{');
        writer.Indent++;
        foreach (var tag in list)
            tag.PrettyPrint(writer);
        writer.Indent--;
        writer.WriteLine('}');
    }
}