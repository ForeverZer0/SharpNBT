using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// A tag the contains a UTF-8 string.
    /// </summary>
    [PublicAPI][Serializable]
    public class StringTag : Tag<string>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StringTag"/> class with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the tag, or <see langword="null"/> if tag has no name.</param>
        /// <param name="value">The value to assign to this tag.</param>
        public StringTag([CanBeNull] string name, [CanBeNull] string value) : base(TagType.String, name, value)
        {
        }
        
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"TAG_String({PrettyName}): \"{Value}\"";

        /// <summary>
        /// Implicit conversion of this tag to a <see cref="string"/>.
        /// </summary>
        /// <param name="tag">The tag to convert.</param>
        /// <returns>The tag represented as a <see cref="string"/>.</returns>
        public static implicit operator string(StringTag tag) => tag.Value;
    }
}