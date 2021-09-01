using System;
using System.Text.RegularExpressions;

namespace SharpNBT.SNBT
{

    internal delegate string ResultHandler(Match match);

    internal class LexerRule
    {
        private readonly ResultHandler processResult;
        
        public TokenType Type { get; }
        
        public Regex Pattern { get; }
        
        public bool IsSkipped { get; }
        
        
        public LexerRule(TokenType type, string pattern, bool skipped = false) : this(type, pattern, null, skipped)
        {
        }
        
        public LexerRule(TokenType type, string pattern, ResultHandler handler, bool skipped = false)
        {
            Type = type;
            Pattern = new Regex(pattern);
            IsSkipped = skipped;
            processResult = handler;
        }

        public string Process(string source, int index, Match match)
        {
            return processResult is null ? source.Substring(index, match.Length) : processResult.Invoke(match);
        }
    }
}