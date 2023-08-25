using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SharpNBT.SNBT;

public ref struct Scanner
{
    public ReadOnlySpan<char> Source;
    public int Position;
    
    public char Current => Source[Position];

    public bool IsEndOfInput => Position >= Source.Length;
    
    public Scanner(ReadOnlySpan<byte> utf8Bytes)
    {
        Position = -1;
        var count = Encoding.UTF8.GetCharCount(utf8Bytes);
        var chars = new char[count];
        Encoding.UTF8.GetChars(utf8Bytes, chars);
        Source = new ReadOnlySpan<char>(chars);
    }

    public char Peek(int numChars = 1)
    {
        if (Position + numChars >= Source.Length)
            SyntaxError("Unexpected end of input.");
        return Source[Position + numChars];
    }
    
    public bool MoveNext(bool skipWhitespace, bool fail)
    {
        ReadChar:
        Position++;
        if (Position >= Source.Length)
        {
            if (fail)
                SyntaxError("Unexpected end of input.");
            return false;
        }
        
        if (skipWhitespace && char.IsWhiteSpace(Current))
            goto ReadChar;
        return true;
    }

    public void AssertChar(char c)
    {
        if (Current != c)
            SyntaxError($"Expected \"{c}\", got \"{Current}\".");
    }

    [DoesNotReturn]
    public Exception SyntaxError(string message)
    {
        // TODO: Format with index,column, etc?
        throw new SyntaxErrorException(message);
    }
}