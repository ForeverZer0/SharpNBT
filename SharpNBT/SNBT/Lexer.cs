using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Security;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

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
        Char,
        EscapedChar
    }


    internal sealed class LexerRule
    {

        internal delegate string PostProcessHandler(Match match);
        
        public Regex Matcher { get; }
        
        public TokenType Type { get; }
        
        public string Description { get; }

        public string PostProcess(Match match) => handler?.Invoke(match) ?? match.Value;

        private readonly PostProcessHandler handler;
        
        public LexerRule(TokenType type, string description, string pattern, [CanBeNull] PostProcessHandler process)
        {
            Type = type;
            Description = description;
            Matcher = new Regex(pattern, RegexOptions.Multiline | RegexOptions.CultureInvariant);
            handler = process;
        }

        // public LexerRule(TokenType type, string description, Regex regex)
        // {
        //     Description = description;
        //     Type = type;
        //     
        // }
    }
    
    internal sealed class Token
    {
        public TokenType Type { get; }
        
        public string Match { get; }

        public Token(TokenType type, string match)
        {
            Type = type;
            Type = type;
            Match = match;
        }
    }

    internal class Lexer
    {
        private static readonly string DoubleQuoteIdentifier = "\"(.*?)\"\\s*(?=:)";

        private static readonly List<LexerRule> rules;
        

        private const string IDENTIFIER_DOUBLE_QUOTES = "\".*?\"\\s*(?>:)";
        private const string IDENTIFIER_SINGLE_QUOTES = "'.*?'\\s*(?>:)";
        private const string IDENTIFIER_NO_QUOTES = @"[A-Za-z0-9_-]+\s*(?=:)";

        private const string STRING_DOUBLE_QUOTED = "^\\s*\".*?\"";
        private const string STRING_SINGLE_QUOTED = "^\\s*'.*?'";

        private const string COMPOUND_START = "\\s*{\\s*";
        private const string COMPOUND_END = @"\}";


        private const string SEPARATOR = "^\\s*:\\s*";
        private const string COMMA = "^\\s*,\\s*";
        
        
        static Lexer()
        {
            rules = new List<LexerRule>
            {
                new LexerRule(TokenType.CompoundBegin, "Opening Compound brace", "^{", null),
                new LexerRule(TokenType.WhiteSpace, "Useless whitespace", @"^[\s]+", null),
                
                new LexerRule(TokenType.Identifier, "Single-quoted name",  "^\\s*'(.*?)'\\s*(?=:)", m => m.Groups[1].Value),
                new LexerRule(TokenType.Identifier, "Double-quoted name",  "^\\s*\"(.*?)\"\\s*(?=:)", m => m.Groups[1].Value),
                new LexerRule(TokenType.Identifier, "Unquoted name",  "^\\s*([A-Za-z0-9_-]+)\\s*(?=:)", m => m.Groups[1].Value),
                
                
                new LexerRule(TokenType.String, "Double-quoted string value", "^\"(.*?)\"", null),
                new LexerRule(TokenType.String, "Single-quoted string value", "^'(.*?)'", null)

                // new LexerRule(TokenType.CompoundBegin, COMPOUND_START),
                // new LexerRule(TokenType.CompoundEnd, COMPOUND_END),
                // new LexerRule(TokenType.Identifier, IDENTIFIER_DOUBLE_QUOTES),
                // new LexerRule(TokenType.Identifier, IDENTIFIER_SINGLE_QUOTES),
                // new LexerRule(TokenType.Identifier, IDENTIFIER_NO_QUOTES),
                // new LexerRule(TokenType.String, STRING_DOUBLE_QUOTED),
                // new LexerRule(TokenType.String, STRING_SINGLE_QUOTED),
                // new LexerRule(TokenType.Separator, SEPARATOR),
                // new LexerRule(TokenType.Comma, COMMA),
                // new LexerRule(TokenType.ByteArray, @"\[B;[\s]*?"),
                // new LexerRule(TokenType.IntArray, @"\[I;[\s]*?"),
                // new LexerRule(TokenType.LongArray, @"\[L;[\s]*?"),
                // new LexerRule(TokenType.ListArray, @"\[[\s]*?"),
                // new LexerRule(TokenType.EndArray, @"[\s]*\]"),
                // new LexerRule(TokenType.Float, @"-?[0-9]*\.[0-9]+[Ff]"),
                // new LexerRule(TokenType.Double, @"-?[0-9]*\.[0-9]+[Dd]?"),
                // new LexerRule(TokenType.Byte, "-?([0-9]+)[Bb]"),
                // new LexerRule(TokenType.Short, "-?([0-9]+)[Ss]"),
                // new LexerRule(TokenType.Long, "-?([0-9]+)[Ll]"),
                // new LexerRule(TokenType.Int, "-?([0-9]+)"),
                // new LexerRule(TokenType.WhiteSpace, @"[\s]+"),
                // new LexerRule(TokenType.String, @"[\S]+"),
                // new LexerRule(TokenType.Char, ".")
            };
        }

        private static string Process(Match match)
        {
            throw new NotImplementedException();
        }

        public Lexer()
        {
            
        }
        
        public IEnumerable<Token> Tokenize(string input)
        {
            string.Create(input.Length, input, (span, i) =>
            {
                
            });
            var pos = 0;

            do
            {
                Label:
                foreach (var rule in rules)
                {
                    var match = rule.Matcher.Match(input, pos);
                    if (match.Success)
                    {
                        yield return new Token(rule.Type, rule.PostProcess(match));
                        pos = match.Index + match.Length - 1;
                        break;
                    }
                }
            } while (++pos < input.Length);
            
        }
    }
}