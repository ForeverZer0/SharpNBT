using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace SharpNBT.SNBT
{
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
            lexer.AddRule(TokenType.Byte, "(-?[0-9]+)[Bb]", FirstGroupValue);
            lexer.AddRule(TokenType.Short, "(-?[0-9]+)[Ss]", FirstGroupValue);
            lexer.AddRule(TokenType.Long, "(-?[0-9]+)[Ll]", FirstGroupValue);
            lexer.AddRule(TokenType.Int, "(-?[0-9]+)", FirstGroupValue);
        }
        
        private static string FirstGroupValue(Match match) => match.Groups[1].Value;
        
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
    }
}