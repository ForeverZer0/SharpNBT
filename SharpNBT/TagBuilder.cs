using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Provides a mechanism for easily building a tree of NBT objects by handling the intermediate step of creating tags, allowing the direct adding of their
/// equivalent values.
/// <para/>
/// All methods return the <see cref="TagBuilder"/> instance itself, allowing for easily chaining calls to build a document.
/// </summary>
[PublicAPI]
public class TagBuilder
{
    private readonly CompoundTag root;
    private readonly Stack<TagContainer> tree;

    /// <summary>
    /// Gets the zero-based depth of the current node, indicating how deeply nested it is within other tags.
    /// </summary>
    /// <remarks>The implicit top-level <see cref="CompoundTag"/> is not factored into this value.</remarks>
    public int Depth => tree.Count - 1;

    /// <summary>
    /// Creates a new instance of the <see cref="TagBuilder"/> class, optionally with a <paramref name="name"/> to assign the top-level
    /// <see cref="CompoundTag"/> of the final result.
    /// </summary>
    /// <param name="name"></param>
    public TagBuilder(string? name = null)
    {
        root = new CompoundTag(name);
        tree = new Stack<TagContainer>();
        tree.Push(root);
    }

    /// <summary>
    /// Adds a new <see cref="ByteTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddBool(string? name, bool value) => AddTag(new ByteTag(name, value));
        
    /// <summary>
    /// Adds a new unnamed <see cref="ByteTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddBool(bool value) => AddBool(null, value);
        
    /// <summary>
    /// Adds a new <see cref="ByteTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddByte(string? name, byte value) => AddTag(new ByteTag(name, value));
    
    /// <inheritdoc cref="AddByte(string,byte)"/>
    public TagBuilder AddByte(string? name, int value) => AddByte(name, unchecked((byte)(value & 0xFF)));
    
    /// <inheritdoc cref="AddByte(string,byte)"/>
    [CLSCompliant(false)]
    public TagBuilder AddByte(string? name, sbyte value) => AddByte(name, unchecked((byte)value));

    /// <summary>
    /// Adds a new unnamed <see cref="ByteTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddByte(byte value) => AddByte(null, value);

    /// <inheritdoc cref="AddByte(sbyte)"/>
    public TagBuilder AddByte(int value) => AddByte(null, unchecked((byte)(value & 0xFF)));
    
    /// <inheritdoc cref="AddByte(sbyte)"/>
    [CLSCompliant(false)]
    public TagBuilder AddByte(sbyte value) => AddByte(null, unchecked((byte)value));

    /// <summary>
    /// Adds a new <see cref="ShortTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddShort(string? name, short value) => AddTag(new ShortTag(name, value));

    /// <inheritdoc cref="AddShort(string,short)"/>
    public TagBuilder AddShort(string? name, int value) => AddShort(name, unchecked((short)(value & 0xFFFF)));
    
    /// <inheritdoc cref="AddShort(string,short)"/>
    [CLSCompliant(false)]
    public TagBuilder AddShort(string? name, ushort value) => AddShort(name, unchecked((short)value));

    /// <summary>
    /// Adds a new unnamed <see cref="ShortTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddShort(short value) => AddShort(null, value);

    /// <inheritdoc cref="AddShort(short)"/>
    public TagBuilder AddShort(int value) => AddShort(null, unchecked((short)(value & 0xFFFF)));
    
    /// <inheritdoc cref="AddShort(short)"/>
    [CLSCompliant(false)]
    public TagBuilder AddShort(ushort value) => AddShort(null, unchecked((short)value));
        
    /// <summary>
    /// Adds a new <see cref="IntTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddInt(string? name, int value) => AddTag(new IntTag(name, value));

    /// <inheritdoc cref="AddInt(string,int)"/>
    [CLSCompliant(false)]
    public TagBuilder AddInt(string? name, uint value) => AddInt(name, unchecked((int)value));

    /// <summary>
    /// Adds a new unnamed <see cref="IntTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddInt(int value) => AddInt(null, value);

    /// <inheritdoc cref="AddInt(int)"/>
    [CLSCompliant(false)]
    public TagBuilder AddInt(uint value) => AddInt(null, unchecked((int)value));
        
    /// <summary>
    /// Adds a new <see cref="LongTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddLong(string? name, long value) => AddTag(new LongTag(name, value));

    /// <inheritdoc cref="AddLong(string,long)"/>
    [CLSCompliant(false)]
    public TagBuilder AddLong(string? name, ulong value) => AddLong(name, unchecked((long)value));

    /// <summary>
    /// Adds a new unnamed <see cref="LongTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddLong(long value) => AddLong(null, value);

    /// <inheritdoc cref="AddLong(long)"/>
    [CLSCompliant(false)]
    public TagBuilder AddLong(ulong value) => AddLong(null, unchecked((long)value));
        
    /// <summary>
    /// Adds a new <see cref="FloatTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddFloat(string? name, float value) => AddTag(new FloatTag(name, value));
        
    /// <summary>
    /// Adds a new unnamed <see cref="FloatTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddFloat(float value) => AddFloat(null, value);

    /// <summary>
    /// Adds a new <see cref="DoubleTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddDouble(string? name, double value) => AddTag(new DoubleTag(name, value));
        
    /// <summary>
    /// Adds a new unnamed <see cref="DoubleTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddDouble(double value) => AddDouble(null, value);
        
    /// <summary>
    /// Adds a new <see cref="StringTag"/> with the specified <paramref name="name"/> and <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="value">The value of the tag.</param>
    public TagBuilder AddString(string? name, string? value) => AddTag(new StringTag(name, value));
        
    /// <summary>
    /// Adds a new unnamed <see cref="StringTag"/> with the specified <paramref name="value"/> to the tree at the current depth.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddString(string? value) => AddString(null, value);
        
    /// <summary>
    /// Adds a new <see cref="ByteArrayTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddByteArray(string? name, params byte[] values) => AddTag(new ByteArrayTag(name, new ReadOnlySpan<byte>(values)));

    /// <inheritdoc cref="AddByteArray(string,byte[])"/>
    public TagBuilder AddByteArray(string? name, IEnumerable<byte> values) => AddByteArray(name, values.ToArray());

    /// <summary>
    /// Adds a new unnamed <see cref="StringTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddByteArray(params byte[] values) => AddByteArray(null, values);

    /// <inheritdoc cref="AddByteArray(byte[])"/>
    public TagBuilder AddByteArray(IEnumerable<byte> values) => AddByteArray(null, values.ToArray());

    /// <summary>
    /// Adds a new <see cref="ByteArrayTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    [CLSCompliant(false)]
    public TagBuilder AddByteArray(string? name, params sbyte[] values)
    {
        var span = new ReadOnlySpan<sbyte>(values);
        return AddTag(new ByteArrayTag(name, MemoryMarshal.Cast<sbyte, byte>(span)));
    }
        
    /// <inheritdoc cref="AddByteArray(string,byte[])"/>
    [CLSCompliant(false)]
    public TagBuilder AddByteArray(string? name, IEnumerable<sbyte> values) => AddByteArray(name, values.ToArray());

    /// <summary>
    /// Adds a new unnamed <see cref="StringTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    [CLSCompliant(false)]
    public TagBuilder AddByteArray(params sbyte[] values) => AddByteArray(null, values);

    /// <inheritdoc cref="AddByteArray(byte[])"/>
    [CLSCompliant(false)]
    public TagBuilder AddByteArray(IEnumerable<sbyte> values) => AddByteArray(null, values.ToArray());
    
    /// <summary>
    /// Adds a new <see cref="IntArrayTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddIntArray(string? name, params int[] values) => AddTag(new IntArrayTag(name, values as IEnumerable<int>));

    /// <inheritdoc cref="AddIntArray(string,int[])"/>
    public TagBuilder AddIntArray(string? name, IEnumerable<int> values) => AddIntArray(name, values.ToArray());

    /// <summary>
    /// Adds a new unnamed <see cref="IntArrayTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddIntArray(params int[] values) => AddIntArray(null, values);

    /// <inheritdoc cref="AddIntArray(int[])"/>
    public TagBuilder AddIntArray(IEnumerable<int> values) => AddIntArray(null, values.ToArray());

    /// <summary>
    /// Adds a new <see cref="LongArrayTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddLongArray(string? name, params long[] values) => AddTag(new LongArrayTag(name, new ReadOnlySpan<long>(values)));

    /// <inheritdoc cref="AddLongArray(string,long[])"/>
    public TagBuilder AddLongArray(string? name, IEnumerable<long> values) => AddLongArray(name, values.ToArray());

    /// <summary>
    /// Adds a new unnamed <see cref="LongArrayTag"/> with the specified <paramref name="values"/> to the tree at the current depth.
    /// </summary>
    /// <param name="values">The value(s) that will be included in the tag.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    public TagBuilder AddLongArray(params long[] values) => AddLongArray(null, values);

    /// <inheritdoc cref="AddLongArray(long[])"/>
    public TagBuilder AddLongArray(IEnumerable<long> values) => AddLongArray(null, values.ToArray());

    /// <summary>
    /// Adds an existing <see cref="Tag"/> object to the tree at the current depth.
    /// </summary>
    /// <param name="tag">The <see cref="Tag"/> instance to add.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if adding to a <see cref="ListTag"/> node, and the type does not match.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tag"/> is <see langword="null"/>.</exception>
    public TagBuilder AddTag(Tag tag)
    {
        var top = tree.Peek();
        top.Add(tag ?? throw new ArgumentNullException(nameof(tag)));
        return this;
    }
        
    /// <summary>
    /// Opens a new <see cref="ListTag"/> section, increasing the current depth level by one.
    /// </summary>
    /// <param name="childType">The <see cref="TagType"/> of the child items this list will contain.</param>
    /// <param name="name">The name to apply to the <see cref="ListTag"/>, or <see langword="null"/> to omit a name.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    /// <seealso cref="EndList"/>
    public TagBuilder BeginList(TagType childType, string? name = null)
    {
        var list = new ListTag(name, childType);
        AddTag(list);
        tree.Push(list);
        return this;
    }
        
    /// <summary>
    /// Closes the current <see cref="ListTag"/> section and decreases the <see cref="Depth"/> by one. Does nothing if the the current node does not
    /// represent a <see cref="ListTag"/>.
    /// </summary>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    /// <seealso cref="BeginList"/>
    public TagBuilder EndList()
    {
        if (tree.TryPeek(out var result) && result is ListTag)
            tree.Pop();
 
        return this;
    }

    /// <summary>
    /// Opens a new <see cref="CompoundTag"/> section, increasing the current depth level by one.
    /// </summary>
    /// <param name="name">The name to apply to the <see cref="ListTag"/>, or <see langword="null"/> to omit a name.</param>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    /// <seealso cref="EndCompound"/>
    public TagBuilder BeginCompound(string? name = null)
    {
        var compound = new CompoundTag(name);
        AddTag(compound);
        tree.Push(compound);
        return this;
    }

    /// <summary>
    /// Closes the current <see cref="CompoundTag"/> section and decreases the <see cref="Depth"/> by one. Does nothing if the the current node does not
    /// represent a <see cref="CompoundTag"/>.
    /// </summary>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    /// <seealso cref="BeginCompound"/>
    public TagBuilder EndCompound()
    {
        if (tree.Count > 1 && tree.TryPeek(out var result) && result is CompoundTag)
            tree.Pop();
 
        return this;
    }

    /// <summary>
    /// Closes the current <see cref="CompoundTag"/> or <see cref="ListTag"/> section and decreases the <see cref="Depth"/> by one.
    /// </summary>
    /// <returns>Returns this <see cref="TagBuilder"/> instance for chaining.</returns>
    /// <remarks>This method does nothing if the current location is already at the top-level.</remarks>
    public TagBuilder End()
    {
        if ((tree.Peek() is ListTag) || (tree.Count > 1 && tree.Peek() is CompoundTag))
            tree.Pop();
        return this;
    }

    /// <summary>
    /// Closes any open compound/list sections, and returns the result as a <see cref="CompoundTag"/>.
    /// </summary>
    /// <remarks>Invoking this method moves the current <see cref="Depth"/> back to the top-level.</remarks>
    /// <returns>A <see cref="CompoundTag"/> representing the result of this <see cref="TagBuilder"/> tree.</returns>
    public CompoundTag Create()
    {
        tree.Clear();
        tree.Push(root);
        return root;
    }

    /// <summary>
    /// Creates a new <see cref="CompoundTag"/> and pushes it to the current scope level, returning a <see cref="Context"/> object that pulls the current
    /// scope back out one level when disposed.
    /// </summary>
    /// <param name="name">The name to apply to the <see cref="ListTag"/>, or <see langword="null"/> to omit a name.</param>
    /// <returns>A <see cref="Context"/> that will close the <see cref="CompoundTag"/> when disposed.</returns>
    /// <remarks>This is essentially no different than <see cref="BeginCompound"/> and <see cref="EndCompound"/> but can use `using` blocks to distinguish scope.</remarks>
    public Context NewCompound(string? name)
    {
        BeginCompound(name);
        return new Context(tree.Peek(), EndCompound);
    }
        
    /// <summary>
    /// Creates a new <see cref="ListTag"/> and pushes it to the current scope level, returning a <see cref="Context"/> object that pulls the current
    /// scope back out one level when disposed.
    /// </summary>
    /// <param name="childType">The <see cref="TagType"/> of the child items this list will contain.</param>
    /// <param name="name">The name to apply to the <see cref="ListTag"/>, or <see langword="null"/> to omit a name.</param>
    /// <returns>A <see cref="Context"/> that will close the <see cref="ListTag"/> when disposed.</returns>
    /// <remarks>This is essentially no different than <see cref="BeginList"/> and <see cref="EndList"/> but can use `using` blocks to distinguish scope.</remarks>
    public Context NewList(TagType childType, string? name)
    {
        BeginList(childType, name);
        return new Context(tree.Peek(), EndList);
    }

    /// <summary>
    /// Represents the context of a single "level" of depth into a <see cref="TagBuilder"/> AST.
    /// </summary>
    /// <remarks>Implements <see cref="IDisposable"/> to that each node can used with <c>using</c> statements for easily distinguishable scope.</remarks>
    [PublicAPI]
    public class Context: IDisposable 
    {
        internal delegate TagBuilder CloseHandler();
        private readonly CloseHandler closeHandler;
            
        /// <summary>
        /// Gets the top-level tag for this context.
        /// </summary>
        public TagContainer Tag { get; }

        internal Context(TagContainer tag, CloseHandler handler)
        {
            Tag = tag;
            closeHandler = handler;
        }

        /// <summary>Closes this context.</summary>
        public void Dispose()
        {
            Console.WriteLine(Tag);
            closeHandler.Invoke();
        }
    }
}