using System.Text;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Represents the end of <see cref="CompoundTag"/>.
    /// </summary>
    [PublicAPI]
    public sealed class EndTag : Tag
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EndTag"/> class.
        /// </summary>
        public EndTag() : base(TagType.End, null)
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"TAG_End";

        /// <inheritdoc />
        protected internal override void PrettyPrinted(StringBuilder buffer, int level, string indent)
        {
            // Do nothing
        }

        /// <summary>
        /// Gets the <i>string</i> representation of this NBT tag (SNBT).
        /// </summary>
        /// <returns>This NBT tag in SNBT format.</returns>
        /// <seealso href="https://minecraft.fandom.com/wiki/NBT_format#SNBT_format"/>
        public override string Stringify() => string.Empty;
    }
}