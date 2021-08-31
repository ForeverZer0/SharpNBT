using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SharpNBT.SNBT
{
    internal enum TokenType
    {
        None,
        CompoundBegin,
        CompoundEnd,
        Identifier,
        String,
        Separator,
        Comma,
        ByteArray,
        IntArray,
        LongArray,
        ListArray,
        EndArray,
        Float,
        Double,
        Byte,
        Short,
        Long,
        Int,
        WhiteSpace,
        Char
    }

    internal sealed class LexerRule
    {
        internal delegate string PostProcessHandler(string input);
        
        private readonly Regex regex;
        
        public TokenType Type { get; }
        
        public LexerRule(TokenType type, string pattern) : this(type, new Regex(pattern, RegexOptions.Multiline | RegexOptions.CultureInvariant))
        {
        }

        public LexerRule(TokenType type, Regex regex)
        {
            Type = type;
            this.regex = regex ?? throw new ArgumentNullException(nameof(regex));
        }

        public LexerRule(TokenType type, Regex regex, PostProcessHandler handler)
        {
            
        }
    }

    internal class Lexer
    {
        private static readonly List<LexerRule> rules;
        
        static Lexer()
        {
            rules = new List<LexerRule>
            {
                new LexerRule(TokenType.CompoundBegin, @"\{[\s]*"),
                new LexerRule(TokenType.CompoundEnd, @"[\s]*\}"),
                new LexerRule(TokenType.Identifier, "\".+?\"(?=:)"),
                new LexerRule(TokenType.Identifier, "'.+?'(?=:) "),
                new LexerRule(TokenType.Identifier, "A-Za-z0-9_-]+?(?=:) "),
                new LexerRule(TokenType.String, "\".*?\""),
                new LexerRule(TokenType.String, "'.*?'"),
                new LexerRule(TokenType.Separator, @"[\s]*:[\s]*"),
                new LexerRule(TokenType.Comma, @"[\s]*,[\s]*"),
                new LexerRule(TokenType.ByteArray, @"\[B;[\s]*?"),
                new LexerRule(TokenType.IntArray, @"\[I;[\s]*?"),
                new LexerRule(TokenType.LongArray, @"\[L;[\s]*?"),
                new LexerRule(TokenType.ListArray, @"\[[\s]*?"),
                new LexerRule(TokenType.EndArray, @"[\s]*\]"),
                new LexerRule(TokenType.Float, @"-?[0-9]*\.[0-9]+[Ff]"),
                new LexerRule(TokenType.Double, @"-?[0-9]*\.[0-9]+[Dd]?"),
                new LexerRule(TokenType.Byte, "-?([0-9]+)[Bb]"),
                new LexerRule(TokenType.Short, "-?([0-9]+)[Ss]"),
                new LexerRule(TokenType.Long, "-?([0-9]+)[Ll]"),
                new LexerRule(TokenType.Int, "-?([0-9]+)"),
                new LexerRule(TokenType.WhiteSpace, @"[\s]+"),
                new LexerRule(TokenType.String, @"[\S]+"),
                new LexerRule(TokenType.Char, ".")
            };
        }
        
        public Lexer()
        {
            
        }
    }
}