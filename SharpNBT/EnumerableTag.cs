using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

namespace SharpNBT
{
    /// <summary>
    /// Base class for tags that contain a collection of values and can be enumerated.
    /// </summary>
    /// <typeparam name="T">The type of the item the tag contains.</typeparam>
    [PublicAPI]
    public abstract class EnumerableTag<T> : Tag, IList<T>
    {
        /// <summary>
        /// Internal list implementation.
        /// </summary>
        private readonly List<T> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableTag{T}"/>.
        /// </summary>
        /// <param name="type">A constant describing the NBT type for this tag.</param>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        protected EnumerableTag(TagType type, [CanBeNull] string name) : base(type, name)
        {
            list = new List<T>();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableTag{T}"/> with the specified <paramref name="values"/>.
        /// </summary>
        /// <param name="type">A constant describing the NBT type for this tag.</param>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection of values to include in this tag.</param>
        protected EnumerableTag(TagType type, [CanBeNull] string name, [NotNull] IEnumerable<T> values) : base(type, name)
        {
            list = new List<T>(values);
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1.GetEnumerator?view=netcore-5.0">`IEnumerable.GetEnumerator` on docs.microsoft.com</a></footer>
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.IEnumerable.GetEnumerator?view=netcore-5.0">`IEnumerable.GetEnumerator` on docs.microsoft.com</a></footer>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1.Add?view=netcore-5.0">`ICollection.Add` on docs.microsoft.com</a></footer>
        [SuppressMessage("ReSharper", "AnnotationConflictInHierarchy")]
        public void Add([NotNull] T item)
        {
            if (item is Tag child)
                child.Parent = this;
            list.Add(item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="EnumerableTag{T}"/>.
        /// </summary>
        /// <param name="items">A collection containing the items to add.</param>
        public void AddRange([NotNull] [ItemNotNull] IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1.Insert?view=netcore-5.0">`IList.Insert` on docs.microsoft.com</a></footer>
        [SuppressMessage("ReSharper", "AnnotationConflictInHierarchy")]
        public void Insert(int index, [NotNull] T item)
        {
            if (item is Tag child)
                child.Parent = this;
            list.Insert(index, item);
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        /// <returns>The element at the specified index.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1.Item?view=netcore-5.0">`IList.Item` on docs.microsoft.com</a></footer>
        [NotNull]
        public T this[int index]
        {
            get => list[index];
            set
            {
                if (value is Tag child)
                    child.Parent = this;
                list[index] = value;
            }
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1.Clear?view=netcore-5.0">`ICollection.Clear` on docs.microsoft.com</a></footer>
        public void Clear()
        {
            foreach (var item in list)
            {
                if (item is Tag child)
                    child.Parent = null;
            }
            list.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1.Contains?view=netcore-5.0">`ICollection.Contains` on docs.microsoft.com</a></footer>
        public bool Contains(T item) => list.Contains(item);

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1.CopyTo?view=netcore-5.0">`ICollection.CopyTo` on docs.microsoft.com</a></footer>
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1.Remove?view=netcore-5.0">`ICollection.Remove` on docs.microsoft.com</a></footer>
        public bool Remove(T item)
        {
            if (list.Remove(item))
            {
                if (item is Tag child)
                    child.Parent = null;
                return true;
            }
            return false;
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1.Count?view=netcore-5.0">`ICollection.Count` on docs.microsoft.com</a></footer>
        public int Count => list.Count;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1.IsReadOnly?view=netcore-5.0">`ICollection.IsReadOnly` on docs.microsoft.com</a></footer>
        public bool IsReadOnly => false;

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1.IndexOf?view=netcore-5.0">`IList.IndexOf` on docs.microsoft.com</a></footer>
        public int IndexOf(T item) => list.IndexOf(item);

        /// <summary>Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1.RemoveAt?view=netcore-5.0">`IList.RemoveAt` on docs.microsoft.com</a></footer>
        public void RemoveAt(int index)
        {
            if (list[index] is Tag child)
                child.Parent = null;
            list.RemoveAt(index);
        }

        protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
        {
            for (var i = 0; i < level; i++)
                buffer.Append(indent);
            buffer.AppendLine(ToString());
        }
    }
}