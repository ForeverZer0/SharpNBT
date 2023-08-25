using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SharpNBT.Tags;

public class TagEventArgs : EventArgs
{
    public Tag Tag { get; init; }
    
    public TagType Type { get; init; }
}

public class TagHandledEventArgs : HandledEventArgs
{
    public bool IsNamed { get; init; }
    
    public TagType Type { get; init; }
    
    public Tag? Result { get; set; }
}

public interface ITag
{
    public static abstract TagType Type { get; }
    
    string? Name { get; }

    void PrettyPrint();
}

public abstract class Tag : ITag, IEquatable<Tag>
{
    protected const string NoName = "None";
    
    // public static TagType Type { get; }

    static TagType ITag.Type => TagType.End;
    
    public string? Name { get; }
    
    protected Tag(string? name)
    {
        Name = name;
    }

    public void PrettyPrint()
    {
        using var writer = new IndentedTextWriter(Console.Out);
        PrettyPrint(writer);
    }

    public void PrettyPrint(Stream stream)
    {
        using var streamWriter = new StreamWriter(stream, null, -1, true);
        using var writer = new IndentedTextWriter(streamWriter);
        PrettyPrint(writer);
    }

    protected internal virtual void PrettyPrint(IndentedTextWriter writer) => writer.WriteLine(ToString());

    protected string PrettyName => string.IsNullOrWhiteSpace(Name) ? NoName : $"\"{Name}\"";

    public bool Equals(Tag? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Tag)obj);
    }

    public override int GetHashCode() => (Name != null ? Name.GetHashCode() : 0);

    public static bool operator ==(Tag? left, Tag? right) => Equals(left, right);

    public static bool operator !=(Tag? left, Tag? right) => !Equals(left, right);
}

public abstract class ValueTag<T> : Tag, IEquatable<ValueTag<T>>
{
    public T Value { get; }

    protected ValueTag(string? name, T value) : base(name)
    {
        Value = value;
    }

    /// <inheritdoc />
    public bool Equals(ValueTag<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ValueTag<T>)obj);
    }
    
    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Value);

    /// <summary>
    /// Compares two values to determine equality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator ==(ValueTag<T>? left, ValueTag<T>? right) => Equals(left, right);

    /// <summary>
    /// Compares two values to determine inequality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool operator !=(ValueTag<T>? left, ValueTag<T>? right) => !Equals(left, right);

    public static implicit operator T(ValueTag<T> tag) => tag.Value;
}

public class ByteTag : NumericTag<byte>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Byte;

    public bool Bool => Value != 0;
    
    public bool IsBool { get; }
    
    public ByteTag(string? name, bool value) : base(name, value ? (byte) 1 : (byte)0)
    {
        IsBool = true;
    }
    
    public ByteTag(string? name, byte value) : base(name, value)
    {
    }
    
    public ByteTag(string? name, int value) : base(name, unchecked((byte)(value & 0xFF)))
    {
    }
    
    [CLSCompliant(false)]
    public ByteTag(string? name, sbyte value) : base(name, Unsafe.As<sbyte, byte>(ref value))
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        object value = IsBool ? Bool : Value;
        return $"TAG_Byte({PrettyName}): {value}";
    }

    public static implicit operator bool(ByteTag tag) => tag.Value != 0;
}

public class ShortTag : NumericTag<short>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Short;
    
    public ShortTag(string? name, short value) : base(name, value)
    {
    }
    
    public ShortTag(string? name, int value) : base(name, unchecked((short)(value & 0xFFFF)))
    {
    }
    
    [CLSCompliant(false)]
    public ShortTag(string? name, ushort value) : base(name, Unsafe.As<ushort, short>(ref value))
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Short({PrettyName}): {Value}";
}

public class IntTag : NumericTag<int>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Int;
    
    public IntTag(string? name, int value) : base(name, value)
    {
    }
    
    [CLSCompliant(false)]
    public IntTag(string? name, uint value) : base(name, Unsafe.As<uint, int>(ref value))
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Int({PrettyName}): {Value}";
}

public class LongTag : NumericTag<long>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Long;
    
    public LongTag(string? name, long value) : base(name, value)
    {
    }
    
    [CLSCompliant(false)]
    public LongTag(string? name, ulong value) : base(name, Unsafe.As<ulong, long>(ref value))
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Long({PrettyName}): {Value}";
}

public class FloatTag : NumericTag<float>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Float;
    
    public FloatTag(string? name, float value) : base(name, value)
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Float({PrettyName}): {Value:0.0}";
}

public class DoubleTag : NumericTag<double>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Double;
    
    public DoubleTag(string? name, double value) : base(name, value)
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_Double({PrettyName}): {Value:0.0}";
}

public class StringTag : ValueTag<string>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.String;
    
    public StringTag(string? name, string? value) : base(name, value ?? string.Empty)
    {
    }
    
    /// <inheritdoc />
    public override string ToString() => $"TAG_String({PrettyName}): \"{Value}\"";
}



public class ByteArrayTag : ArrayTag<byte>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.ByteArray;
    
    public ByteArrayTag(string? name, byte[] value) : base(name, value)
    {
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_ByteArray({PrettyName}): {Count} {word}";
    }
}

public class IntArrayTag : ArrayTag<int>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.IntArray;
    
    public IntArrayTag(string? name, int[] value) : base(name, value)
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_IntArray({PrettyName}): {Count} {word}";
    }
}

public class LongArrayTag : ArrayTag<long>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.LongArray;
    
    public LongArrayTag(string? name, long[] value) : base(name, value)
    {
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_LongArray({PrettyName}): {Count} {word}";
    }
}

public class CompoundTag : Tag, IDictionary<string, Tag>, ITag
{
    /// <inheritdoc />
    static TagType ITag.Type => TagType.Compound;
    
    private readonly Dictionary<string, Tag> dict;

    public CompoundTag(string? name) : base(name)
    {
        dict = new Dictionary<string, Tag>();
    }

    private Tag AssertName(Tag tag)
    {
        // TODO
        if (string.IsNullOrWhiteSpace(tag.Name))
            throw new ArgumentException("Compound tags require all children to have a valid name.");
        return tag;
    }

    public void Add(Tag tag) => dict.Add(tag.Name!, AssertName(tag));

    public IEnumerator<KeyValuePair<string, Tag>> GetEnumerator() => dict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)dict).GetEnumerator();

    public void Add(KeyValuePair<string, Tag> item) => dict.Add(item.Key, AssertName(item.Value));

    public void Clear() => dict.Clear();

    public bool Contains(KeyValuePair<string, Tag> item) => dict.Contains(item);

    public void CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex)
    {
        foreach (var kvp in dict)
        {
            array[arrayIndex++] = kvp;
        }
    }

    public bool Remove(KeyValuePair<string, Tag> item) => dict.Remove(item.Key);

    public int Count => dict.Count;

    public bool IsReadOnly => false;

    public void Add(string key, Tag value) => dict.Add(key, AssertName(value));

    public bool ContainsKey(string key) => dict.ContainsKey(key);

    public bool Remove(string key) => dict.Remove(key);

    public bool TryGetValue(string key, out Tag value) => dict.TryGetValue(key, out value!);

    public Tag this[string key]
    {
        get => dict[key];
        set => dict[key] = AssertName(value);
    }

    public ICollection<string> Keys => dict.Keys;

    public ICollection<Tag> Values => dict.Values;
    
    /// <inheritdoc />
    public override string ToString()
    {
        var word = Count == 1 ? "entry" : "entries";
        return $"TAG_Compound({PrettyName}): {Count} {word}";
    }

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