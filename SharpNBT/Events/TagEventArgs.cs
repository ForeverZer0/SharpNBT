using System;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Arguments supplied with tag-related events.
    /// </summary>
    public class TagEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a constant describing the basic NBT type of the tag.
        /// </summary>
        public TagType Type { get; }
        
        /// <summary>
        /// Gets the parsed <see cref="Tag"/> instance.
        /// </summary>
        [NotNull]
        public Tag Tag { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="TagEventArgs"/> class.
        /// </summary>
        /// <param name="type">A constant describing the basic NBT type of the tag.</param>
        /// <param name="tag">The parsed <see cref="Tag"/> instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tag"/> is <see langword="null"/>.</exception>
        public TagEventArgs(TagType type, [NotNull] Tag tag)
        {
            Type = type;
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        }
    }
}