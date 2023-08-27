using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Abstract base class for <see cref="Tag"/> types that contain a single numeric value.
/// </summary>
/// <typeparam name="T">A value type that implements <see cref="INumber{TSelf}"/>.</typeparam>
[PublicAPI][Serializable]
public abstract class NumericTag<T> : Tag, IEquatable<NumericTag<T>>, IComparable<NumericTag<T>>, IComparable where T : unmanaged, INumber<T>
{
    public T Value { get; set;  }

    protected NumericTag(TagType type, string? name, T value) : base(type, name)
    {
        Value = value;
    }

    protected NumericTag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        var value = info.GetValue("value", typeof(T));
        Value = value is null ? default : (T)value;
    }

    public bool Equals(NumericTag<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((NumericTag<T>)obj);
    }

    public override int GetHashCode() => base.GetHashCode();

    public int CompareTo(NumericTag<T>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is NumericTag<T> other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(NumericTag<T>)}");
    }
    
    public static bool operator ==(NumericTag<T>? left, NumericTag<T>? right) => Equals(left, right);

    public static bool operator !=(NumericTag<T>? left, NumericTag<T>? right) => !Equals(left, right);

    public static bool operator <(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) < 0;
    }

    public static bool operator >(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) > 0;
    }

    public static bool operator <=(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) <= 0;
    }

    public static bool operator >=(NumericTag<T>? left, NumericTag<T>? right)
    {
        return Comparer<NumericTag<T>>.Default.Compare(left, right) >= 0;
    }
}