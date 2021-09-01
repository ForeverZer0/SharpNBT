using JetBrains.Annotations;

namespace SharpNBT.SNBT
{
    /// <summary>
    /// An object emitted by the lexer to describe a logical fragment of code that can be parsed.
    /// </summary>
    [PublicAPI]
    public sealed class Token
    {
        /// <summary>
        /// Gets a value describing the general type code fragment this <see cref="Token"/> represents.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Gets a value of this fragment, which can vary depending on context and the <see cref="Type"/>.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="type">A value describing the general type code fragment this <see cref="Token"/> represents.</param>
        /// <param name="value">Ahe value of this code fragment.</param>
        public Token(TokenType type,  [NotNull] string value)
        {
            Type = type;
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString() => $"[{Type}] \"{Value}\"";
    }
}