using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT.SNBT
{
    /// <summary>
    /// Provides static methods for parsing string-NBT (SNBT) source text into a complete <see cref="CompoundTag"/>.
    /// </summary>
    [PublicAPI]
    public static class StringNbt
    {
        private static readonly Lexer lexer;

        static StringNbt()
        {
            lexer = new Lexer();
            lexer.AddRule(TokenType.Whitespace, @"(\r|\t|\v|\f|\s)+?", true);
            lexer.AddRule(TokenType.Separator, ",", true);
            lexer.AddRule(TokenType.Compound, @"{");
            lexer.AddRule(TokenType.EndCompound, @"}");
            lexer.AddRule(TokenType.Identifier, "\"(.*?)\"\\s*(?>:)", FirstGroupValue);
            lexer.AddRule(TokenType.Identifier, "'(.*?)'\\s*(?>:)", FirstGroupValue);
            lexer.AddRule(TokenType.Identifier, "([A-Za-z0-9_-]+)\\s*(?>:)", FirstGroupValue);
            lexer.AddRule(TokenType.String, "\"(.*?)\"", FirstGroupValue);
            lexer.AddRule(TokenType.String, "'(.*?)'", FirstGroupValue);
            lexer.AddRule(TokenType.ByteArray, @"\[B;");
            lexer.AddRule(TokenType.IntArray, @"\[I;");
            lexer.AddRule(TokenType.LongArray, @"\[L;");
            lexer.AddRule(TokenType.List, @"\[");
            lexer.AddRule(TokenType.EndArray, @"\]");
            lexer.AddRule(TokenType.Float, @"(-?[0-9]*\.[0-9]+)[Ff]", FirstGroupValue);
            lexer.AddRule(TokenType.Double, @"(-?[0-9]*\.[0-9]+)[Dd]?", FirstGroupValue);
            lexer.AddRule(TokenType.Bool, "(true|false)", FirstGroupValue);
            lexer.AddRule(TokenType.Byte, "(-?[0-9]+)[Bb]", FirstGroupValue);
            lexer.AddRule(TokenType.Short, "(-?[0-9]+)[Ss]", FirstGroupValue);
            lexer.AddRule(TokenType.Long, "(-?[0-9]+)[Ll]", FirstGroupValue);
            lexer.AddRule(TokenType.Int, "(-?[0-9]+)", FirstGroupValue);
        }

        /// <summary>
        /// Parse the text in the given <paramref name="stream"/> into a <see cref="CompoundTag"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> containing the SNBT data.</param>
        /// <param name="length">The number of bytes to read from the <paramref name="stream"/>, advancing its position.</param>
        /// <returns>The <see cref="CompoundTag"/> instance described in the source text.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="IOException">When <paramref name="stream"/> is not opened for reading.</exception>
        /// <exception cref="ArgumentException">When <paramref name="length"/> is negative.</exception>
        /// <exception cref="SyntaxErrorException">When <paramref name="stream"/> contains invalid SNBT code.</exception>
        [NotNull]
        public static CompoundTag Parse(Stream stream, int length)
        {
            Validate(stream, length);
            if (length == 0)
                return new CompoundTag(null);
            
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            var str = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            
            return Parse(str);
        }

        /// <summary>
        /// Asynchronously parses the text in the given <paramref name="stream"/> into a <see cref="CompoundTag"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> containing the SNBT data.</param>
        /// <param name="length">The number of bytes to read from the <paramref name="stream"/>, advancing its position.</param>
        /// <returns>The <see cref="CompoundTag"/> instance described in the source text.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="IOException">When <paramref name="stream"/> is not opened for reading.</exception>
        /// <exception cref="ArgumentException">When <paramref name="length"/> is negative.</exception>
        /// <exception cref="SyntaxErrorException">When <paramref name="stream"/> contains invalid SNBT code.</exception>
        [NotNull]
        public static async Task<CompoundTag> ParseAsync(Stream stream, int length)
        {
            Validate(stream, length);
            if (length == 0)
                return new CompoundTag(null);
            
            var buffer = new byte[length];
            await stream.ReadAsync(buffer, 0, length);
            var str = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            
            return Parse(str);
        }

        private static void Validate(Stream stream, int length)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new IOException("Stream is not opened for reading.");
            
            if (length < 0)
                throw new ArgumentException(Strings.NegativeLengthSpecified, nameof(length));
        }
        
        /// <summary>
        /// Parse the given <paramref name="source"/> text into a <see cref="CompoundTag"/>.
        /// </summary>
        /// <param name="source">A string containing the SNBT code to parse.</param>
        /// <returns>The <see cref="CompoundTag"/> instance described in the source text.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="SyntaxErrorException">When <paramref name="source"/> is invalid SNBT code.</exception>
        [NotNull]
        public static CompoundTag Parse([NotNull] string source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(source))
                return new CompoundTag(null);

            var queue = new Queue<Token>(lexer.Tokenize(source));
            return Parse<CompoundTag>(queue);
        }

        private static T Parse<T>(Queue<Token> queue) where T : Tag => (T)Parse(queue);
        
        private static Tag Parse(Queue<Token> queue)
        {
            string name = null;
            var token = MoveNext(queue);

            if (token.Type == TokenType.Identifier)
            {
                name = token.Value;
                token = MoveNext(queue);
            }

            return token.Type switch
            {
                TokenType.Compound => ParseCompound(name, queue),
                TokenType.String => new StringTag(name, token.Value),
                TokenType.ByteArray => ParseByteArray(name, queue),
                TokenType.IntArray => ParseIntArray(name, queue),
                TokenType.LongArray => ParseLongArray(name, queue),
                TokenType.List => ParseList(name, queue),
                TokenType.Bool => new BoolTag(name, bool.Parse(token.Value)),
                TokenType.Byte => new ByteTag(name, sbyte.Parse(token.Value)),
                TokenType.Short => new ShortTag(name, short.Parse(token.Value)),
                TokenType.Int => new IntTag(name, int.Parse(token.Value)),
                TokenType.Long => new LongTag(name, long.Parse(token.Value)),
                TokenType.Float => new FloatTag(name, float.Parse(token.Value)),
                TokenType.Double => new DoubleTag(name, double.Parse(token.Value)),
                _ => throw new SyntaxErrorException()
            };
        }
        
        [NotNull]
        private static Token MoveNext(Queue<Token> queue)
        {
            if (queue.TryDequeue(out var token))
                return token;
            
            throw new SyntaxErrorException("Unexpected end-of-input");
        }
        
        private static void MoveNext(Queue<Token> queue, TokenType assertType)
        {
            var token = MoveNext(queue);
            if (token.Type != assertType)
                throw new SyntaxErrorException($"Expected token of type {assertType}, but encountered {token.Type}.");
        }

        private static CompoundTag ParseCompound(string name, Queue<Token> queue)
        {
            var compound = new CompoundTag(name);
            while (queue.TryPeek(out var token) && token.Type != TokenType.EndCompound)
            {
                compound.Add(Parse(queue));
            }
            MoveNext(queue, TokenType.EndCompound);
            return compound;
        }
        
        private static ListTag ParseList(string name, Queue<Token> queue)
        {
            var values = new List<Tag>();
            while (queue.TryPeek(out var token) && token.Type != TokenType.EndArray)
            {
                values.Add(Parse(queue));
            }

            MoveNext(queue, TokenType.EndArray);
            if (values.Count > 0)
            {
                var type = values[0].Type;
                return new ListTag(name, type, values);
            }
            return new ListTag(name, TagType.End);
        }
        
        private static ByteArrayTag ParseByteArray(string name, Queue<Token> queue)
        {
            var values = new List<byte>();
            foreach (var token in DequeueUntil(queue, TokenType.EndArray))
            {
                if (token.Type != TokenType.Byte)
                    throw new SyntaxErrorException($"Invalid token type in array, expected {TokenType.Byte}, got {token.Type}.");
                values.Add(unchecked((byte) sbyte.Parse(token.Value)));
            }
            return new ByteArrayTag(name, values);
        }

        private static IntArrayTag ParseIntArray(string name, Queue<Token> queue)
        {
            var values = new List<int>();
            foreach (var token in DequeueUntil(queue, TokenType.EndArray))
            {
                if (token.Type != TokenType.Int)
                    throw new SyntaxErrorException($"Invalid token type in array, expected {TokenType.Int}, got {token.Type}.");
                values.Add(int.Parse(token.Value));
            }
            return new IntArrayTag(name, values);
        }
        
        private static LongArrayTag ParseLongArray(string name, Queue<Token> queue)
        {
            var values = new List<long>();
            foreach (var token in DequeueUntil(queue, TokenType.EndArray))
            {
                if (token.Type != TokenType.Long)
                    throw new SyntaxErrorException($"Invalid token type in array, expected {TokenType.Long}, got {token.Type}.");
                values.Add(long.Parse(token.Value));
            }
            return new LongArrayTag(name, values);
        }

        private static IEnumerable<Token> DequeueUntil(Queue<Token> queue, TokenType type)
        {
            while (true)
            {
                var token = MoveNext(queue);
                if (token.Type == type)
                    yield break;
                yield return token;
            }
        }
        
        private static string FirstGroupValue(Match match) => match.Groups[1].Value;
    }
}