using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// The base class for tag types that represent a contiguous array of numeric values.
/// </summary>
/// <typeparam name="T">A numeric type.</typeparam>
/// <remarks>
/// The type may be used as a drop-in replacement for a standard array type, including range operators and Span-like
/// functionality.
/// </remarks>
[PublicAPI]
public abstract class ArrayTag<T> : Tag, IReadOnlyList<T> where T : unmanaged, INumber<T>
{
    private readonly T[] array;

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the tag's values.
    /// </summary>
    public Span<T> Span => new(array);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayTag{T}"/> class.
    /// </summary>
    /// <param name="name">The optional name of the tag.</param>
    /// <param name="value">The value of the tag payload.</param>
    protected ArrayTag(string? name, T[] value) : base(name)
    {
        array = value;
    }

    public ref T Pin() => ref array[0]; // TODO
    
    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        // for loops are significantly faster than foreach loops, especially with basic arrays.
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < array.Length; i++)
            yield return array[i];
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();

    /// <inheritdoc cref="Span{T}.Slice(int,int)"/>
    public Span<T> Slice(int start, int length) => new(array, start, length);

    /// <inheritdoc cref="ICollection.CopyTo"/>
    // ReSharper disable once ParameterHidesMember
    public void CopyTo(T[] array, int arrayIndex) => this.array.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public int Count => array.Length;

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    public int IndexOf(T item)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(item))
                return i;
        }

        return -1;
    }

    /// <inheritdoc cref="IList{T}.this"/>
    public T this[int index]
    {
        get => array[index];
        set => array[index] = value;
    }

    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to an array of <see cref="T"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The array representation of the <paramref name="tag"/>.</returns>
    public static implicit operator T[](ArrayTag<T> tag) => tag.array;
    
    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to a <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The <see cref="Span{T}"/> representation of the <paramref name="tag"/>.</returns>
    public static implicit operator Span<T>(ArrayTag<T> tag) => new(tag.array);
    
    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The <see cref="ReadOnlySpan{T}"/> representation of the <paramref name="tag"/>.</returns>
    public static implicit operator ReadOnlySpan<T>(ArrayTag<T> tag) => new(tag.array);
    
    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to a <see cref="Memory{T}"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The <see cref="Memory{T}"/> representation of the <paramref name="tag"/>.</returns>
    public static implicit operator Memory<T>(ArrayTag<T> tag) => new(tag.array);
    
    /// <summary>
    /// Implicit conversion of a an <see cref="ArrayTag{T}"/> to a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ArrayTag{T}"/> to be converted.</param>
    /// <returns>The <see cref="ReadOnlyMemory{T}"/> representation of the <paramref name="tag"/>.</returns>
    public static implicit operator ReadOnlyMemory<T>(ArrayTag<T> tag) => new(tag.array);
}