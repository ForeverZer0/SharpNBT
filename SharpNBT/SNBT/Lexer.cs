using System.Collections.Generic;
using System.Data;

namespace SharpNBT.SNBT
{
    internal sealed class Lexer
    {
        private readonly List<LexerRule> ruleList;
        
        public Lexer()
        {
            ruleList = new List<LexerRule>();
        }
        
        public void AddRule(TokenType type, string pattern, bool skipped = false) => ruleList.Add(new LexerRule(type, pattern, null, skipped));

        public void AddRule(TokenType type, string pattern, ResultHandler handler, bool skipped = false)
        {
            ruleList.Add(new LexerRule(type, pattern, handler, skipped));
        }
        
        public IEnumerable<Token> Tokenize(string source)
        {
            var index = 0;
            while (index < source.Length)
            {
                var success = false;

                foreach (var rule in ruleList)
                {
                    var match = rule.Pattern.Match(source, index);
                    if (!match.Success || match.Index - index != 0) 
                        continue;
                    
                    if (!rule.IsSkipped)
                        yield return new Token(rule.Type, rule.Process(source, index, match));

                    index += match.Length;
                    success = true;
                    break;
                }

                if (!success)
                    throw new SyntaxErrorException($"Unrecognized sequence at index {index}: '{source[index]}'");
            }
        }
        
        
    }
}