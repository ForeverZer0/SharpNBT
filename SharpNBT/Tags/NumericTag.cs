using System;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Abstract base class for NBT tags that represent a primitive numeric value.
/// </summary>
/// <typeparam name="T">A numeric type.</typeparam>
[PublicAPI]
public abstract class NumericTag<T> : Tag, IEquatable<NumericTag<T>>, IComparable<NumericTag<T>>, IComparable where T : unmanaged, INumber<T>
{
    /// <summary>
    /// Gets the value of the tag.
    /// </summary>
    public T Value { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericTag{T}"/> class.
    /// </summary>
    /// /// <param name="name">The optional name of the tag.</param>
    /// <param name="value">The numeric value of the tag.</param>
    protected NumericTag(string? name, T value) : base(name)
    {
        Value = value;
    }
    
    /// <inheritdoc />
    public bool Equals(NumericTag<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((NumericTag<T>)obj);
    }
    
    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Value);
    
    /// <inheritdoc />
    public int CompareTo(NumericTag<T>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return ReferenceEquals(null, other) ? 1 : Value.CompareTo(other.Value);
    }

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is NumericTag<T> other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(NumericTag<T>)}");
    }

    /// <summary>
    /// Compares two values to determine equality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator ==(NumericTag<T>? left, NumericTag<T>? right) => Equals(left, right);

    /// <summary>
    /// Compares two values to determine inequality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator !=(NumericTag<T>? left, NumericTag<T>? right) => !Equals(left, right);
    
    /// <summary>Compares two values to determine which is less.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is less than <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator <(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) < 0;
    }

    /// <summary>Compares two values to determine which is greater.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator >(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) > 0;
    }

    /// <summary>Compares two values to determine which is less or equal.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is less than or equal to <paramref name="right" />;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator <=(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) <= 0;
    }

    /// <summary>Compares two values to determine which is greater or equal.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is greater than or equal to <paramref name="right" />;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator >=(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) >= 0;
    }
    
    /// <summary>
    /// Implicit conversion of a an <see cref="NumericTag{T}"/> to a <see cref="T"/>.
    /// </summary>
    /// <param name="tag">The <see cref="NumericTag{T}"/> to be converted.</param>
    /// <returns>The value of <paramref name="tag"/> as a <see cref="T"/>.</returns>
    public static implicit operator T(NumericTag<T> tag) => tag.Value;
}