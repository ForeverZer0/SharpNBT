using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Represents a collection of NBT tags.
/// </summary>
[PublicAPI]
public interface IListTag : IList<Tag>, ITag
{
    /// <summary>
    /// Gets a constant describing the type of child NBT tag in the collection.
    /// </summary>
    public static abstract TagType ChildType { get; }

    public static IListTag Create(string? name, TagType childType)
    {
        return childType switch
        {
            TagType.End => new ListTag<EndTag>(null),
            TagType.Byte => new ListTag<ByteTag>(name),
            TagType.Short => new ListTag<ShortTag>(name),
            TagType.Int => new ListTag<IntTag>(name),
            TagType.Long => new ListTag<LongTag>(name),
            TagType.Float => new ListTag<FloatTag>(name),
            TagType.Double => new ListTag<DoubleTag>(name),
            TagType.ByteArray => new ListTag<ByteArrayTag>(name),
            TagType.String => new ListTag<StringTag>(name),
            TagType.List => new ListTag<IListTag>(name),
            TagType.Compound => new ListTag<CompoundTag>(name),
            TagType.IntArray => new ListTag<IntArrayTag>(name),
            TagType.LongArray => new ListTag<LongArrayTag>(name),
            _ => throw new ArgumentOutOfRangeException(nameof(childType), childType, null)
        }
    }
}

/// <summary>
/// A type-safe collection of <b>nameless</b> NBT tags.
/// </summary>
/// <typeparam name="TTag">A <see cref="Tag"/> type that implements <see cref="ITag"/>.</typeparam>
[PublicAPI]
public class ListTag<TTag> :  Tag, IList<TTag>, IListTag where TTag : Tag, ITag
{
    /// <inheritdoc />
    public static TagType Type => TagType.List;
    
    /// <inheritdoc />
    public static TagType ChildType => TTag.Type;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag{TTag}"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    /// <param name="values">A collection of values to add to the list.</param>
    public ListTag(string? name, IEnumerable<TTag> values) : base(name)
    {
        list = new List<TTag>(values);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag{TTag}"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    public ListTag(string? name) : base(name)
    {
        list = new List<TTag>();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTag{TTag}"/> class.
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
    IEnumerator<Tag> IEnumerable<Tag>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add(TTag item) => list.Add(item);

    /// <inheritdoc />
    void ICollection<Tag>.Add(Tag item) => Add((TTag)item);
    
    /// <inheritdoc />
    public void CopyTo(TTag[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
    
    /// <inheritdoc />
    void ICollection<Tag>.CopyTo(Tag[] array, int arrayIndex)
    {
        for (var i = 0; i < Count; i++)
            array[arrayIndex++] = list[i];
    }
    
    /// <inheritdoc />
    public bool Contains(TTag item) => list.Contains(item);

    /// <inheritdoc />
    bool ICollection<Tag>.Contains(Tag item) => Contains((TTag)item);
    
    /// <inheritdoc />
    public void Insert(int index, TTag item) => list.Insert(index, item);
    
    /// <inheritdoc />
    void IList<Tag>.Insert(int index, Tag item) => list.Insert(index, (TTag)item);

    /// <inheritdoc />
    public int IndexOf(TTag item) => list.IndexOf(item);
    
    /// <inheritdoc />
    int IList<Tag>.IndexOf(Tag item) => list.IndexOf((TTag)item);
    
    /// <inheritdoc />
    public bool Remove(TTag item) => list.Remove(item);
    
    /// <inheritdoc />
    bool ICollection<Tag>.Remove(Tag item) => list.Remove((TTag)item);
    
    /// <inheritdoc cref="IList{T}.RemoveAt(int)"/>
    public void RemoveAt(int index) => list.RemoveAt(index);
    
    /// <inheritdoc cref="IList{T}.Clear"/>
    public void Clear() => list.Clear();
    
    public int Count => list.Count;

    bool ICollection<Tag>.IsReadOnly => false;

    bool ICollection<TTag>.IsReadOnly => false;
    
    Tag IList<Tag>.this[int index]
    {
        get => list[index];
        set => list[index] = (TTag)value;
    }

    public TTag this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }
    
    private readonly List<TTag> list;
}