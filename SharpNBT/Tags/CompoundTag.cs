using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Top-level tag that acts as a container for other <b>named</b> tags. 
    /// </summary>
    /// <remarks>
    /// This along with the <see cref="ListTag"/> class define the structure of the NBT format. Children are not order-dependent, nor is order guaranteed. The
    /// closing <see cref="EndTag"/> does not require to be explicitly added, it will be added automatically during serialization. 
    /// </remarks>
    [PublicAPI][Serializable]
    public class CompoundTag :  TagContainer
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CompoundTag"/> class.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        public CompoundTag([CanBeNull] string name) : base(TagType.Compound, name)
        {
            NamedChildren = true;
            RequiredType = null;
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="CompoundTag"/> class.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection <see cref="Tag"/> objects that are children of this object.</param>
        public CompoundTag([CanBeNull] string name, [NotNull] IEnumerable<Tag> values) : this(name)
        {
            AddRange(values);
        }
        
        /// <summary>
        /// Required constructor for ISerializable implementation.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to describing this instance.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        protected CompoundTag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.ToString?view=netcore-5.0">`Object.ToString` on docs.microsoft.com</a></footer>
        public override string ToString()
        {
            var word = Count == 1 ? Strings.WordEntry : Strings.WordEntries;
            return $"TAG_Compound({PrettyName}): [{Count} {word}]";
        }

        /// <summary>
        /// Retrieves a "pretty-printed" multiline string representing the complete tree structure of the tag.
        /// </summary>
        /// <param name="indent">The prefix that will be applied to each indent-level of nested nodes in the tree structure.</param>
        /// <returns>The pretty-printed string.</returns>
        [NotNull]
        public string PrettyPrinted([CanBeNull] string indent = "    ")
        {
            var buffer = new StringBuilder();
            PrettyPrinted(buffer, 0, indent ?? string.Empty);
            return buffer.ToString();
        }

        /// <summary>
        /// Searches the children of this tag, returning the first child with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the tag to search for.</param>
        /// <param name="deep"><see langword="true"/> to recursively search children, otherwise <see langword="false"/> to only search direct descendants.</param>
        /// <returns>The first tag found with <paramref name="name"/>, otherwise <see langword="null"/> if none was found.</returns>
        [CanBeNull]
        public Tag Find([NotNull] string name, bool deep)
        {
            foreach (var tag in this)
            {
                if (string.CompareOrdinal(name, tag.Name) == 0)
                    return tag;

                if (deep && tag is CompoundTag child)
                {
                    var result = child.Find(name, true);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves a child tag with the specified <paramref name="name"/>, or <see langword="null"/> if no match was found.
        /// </summary>
        /// <param name="name">The name of the tag to retrieve.</param>
        [CanBeNull] 
        public Tag this[[NotNull] string name] => Find(name, false);

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
    }
}