using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Represents a collection of a tags.
    /// </summary>
    /// <remarks>
    /// All child tags <b>must</b> be have the same <see cref="Tag.Type"/> value, and their <see cref="Tag.Name"/> value will be omitted during serialization.
    /// </remarks>
    [PublicAPI][Serializable]
    public class ListTag : TagContainer
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
        public ListTag([CanBeNull] string name, TagType childType) : base(TagType.List, name)
        {
            RequiredType = childType;
            NamedChildren = false;
            ChildType = childType;
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="ListTag"/> with the specified <paramref name="children"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="childType">A constant describing the NBT type for children in this tag.</param>
        /// <param name="children">A collection of values to include in this tag.</param>
        public ListTag([CanBeNull] string name, TagType childType, [NotNull][ItemNotNull] IEnumerable<Tag> children) : this(name, childType)
        {
            AddRange(children);
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            var word = Count == 1 ? "entry" : "entries";
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
        [NotNull]
        public string PrettyPrinted([NotNull] string indent = "    ")
        {
            var buffer = new StringBuilder();
            PrettyPrinted(buffer, 0, indent);
            return buffer.ToString();
        }
    }
}