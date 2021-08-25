using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Represents the end of <see cref="CompoundTag"/>.
    /// </summary>
    [PublicAPI]
    public class EndTag : Tag
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EndTag"/> class.
        /// </summary>
        public EndTag() : base(TagType.End, null)
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"TAG_End";

        protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
        {
            // Do nothing
        }
    }
}