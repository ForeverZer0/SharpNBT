using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Base class for NBT tags that contain a fixed-size array of numeric types.
/// </summary>
/// <typeparam name="T">A value type that implements <see cref="INumber{TSelf}"/>.</typeparam>
[PublicAPI][Serializable]
public abstract class ArrayTag<T> : Tag, IReadOnlyList<T> where T : unmanaged, INumber<T>
{
    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the tag data.
    /// </summary>
    public Span<T> Span => new(array);

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> over the tag data.
    /// </summary>
    public Memory<T> Memory => new(array);
    
    /// <inheritdoc />
    /// <param name="value">The value of the tag.</param>
    // ReSharper disable InvalidXmlDocComment
    protected ArrayTag(TagType type, string? name, T[] value) : base(type, name)
    // ReSharper restore InvalidXmlDocComment
    {
        array = value;
    }

    /// <inheritdoc />
    protected ArrayTag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        var _ = info.GetInt32("count");
        var value = info.GetValue("values", typeof(T[])) as T[];
        array = value ?? Array.Empty<T>();
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("count", array.Length);
        info.AddValue("values", array);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        // ReSharper disable ForCanBeConvertedToForeach
        for (var i = 0; i < array.Length; i++)
            yield return array[i];
        // ReSharper restore ForCanBeConvertedToForeach
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();

    /// <inheritdoc cref="IList{T}.CopyTo"/>
    // ReSharper disable once ParameterHidesMember
    public void CopyTo(T[] array, int arrayIndex) => this.array.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public int Count => array.Length;

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    public int IndexOf(T item)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (array[i] == item)
                return i;
        }

        return -1;
    }

    /// <inheritdoc cref="Span{T}.Slice(int,int)"/>
    /// <remarks>This method being defined provides Range indexers for the class.</remarks>
    public Span<T> Slice(int start, int length) => new(array, start, length);
    
    /// <inheritdoc cref="IList{T}.this"/>
    public T this[int index]
    {
        get => array[index];
        set => array[index] = value;
    }

    /// <summary>
    /// Returns a reference to the underlying memory of this object that is be pinned using the <see langword="fixed"/>
    /// statement.
    /// </summary>
    /// <returns>A reference to the first value in the underlying array.</returns>
    public ref T GetPinnableReference() => ref array[0] ;
    
    private protected string Stringify(char prefix, char? suffix)
    {
        var sb = new StringBuilder(32 + array.Length * 4);
        sb.Append($"{StringifyName}:[{prefix};");

        for (var i = 0; i < array.Length; i++)
        {
            if (i > 0)
                sb.Append(',');
            sb.Append(array[i]);
            if (suffix != null)
                sb.Append(suffix.Value);
        }
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to an array of <see cref="T"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as an array of <see cref="T"/>.</returns>
    public static implicit operator T[](ArrayTag<T> tag) => tag.array;

    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to a <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="Span{T}"/>.</returns>
    public static implicit operator Span<T>(ArrayTag<T> tag) => new(tag.array);
    
    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to a <see cref="Memory{T}"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="Memory{T}"/>.</returns>
    public static implicit operator Memory<T>(ArrayTag<T> tag) => new(tag.array);
    
    private readonly T[] array;
}