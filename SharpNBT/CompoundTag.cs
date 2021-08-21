using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Top-level tag that acts as a container for other <b>named</b> tags.
    /// </summary>
    /// <remarks>
    /// This along with the <see cref="ListTag"/> class define the structure of the NBT format. Children are not order-dependent, nor is order guaranteed.
    /// </remarks>
    [PublicAPI]
    public class CompoundTag : EnumerableTag<Tag>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CompoundTag"/> class.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        public CompoundTag([CanBeNull] string name) : base(TagType.Compound, name)
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="CompoundTag"/> class.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="values">A collection <see cref="Tag"/> objects that are children of this object.</param>
        public CompoundTag([CanBeNull] string name, [NotNull] IEnumerable<Tag> values) : base(TagType.Compound, name, values)
        {
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.ToString?view=netcore-5.0">`Object.ToString` on docs.microsoft.com</a></footer>
        public override string ToString()
        {
            var word = Count == 1 ? "entry" : "entries";
            return $"TAG_Compound({PrettyName}): [{Count} {word}]";
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
                if (name.Equals(tag.Name))
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