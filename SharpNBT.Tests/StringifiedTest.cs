using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using SharpNBT.SNBT;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class StringifiedTest
    {
        private readonly ITestOutputHelper output;
        
        
        public StringifiedTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void BigOutput()
        {
            var tag = TestHelper.GetTag("bigtest.nbt", CompressionType.GZip);
            output.WriteLine(tag.Stringify(true));
        }

        [Fact]
        public void HelloWorldOutput()
        {
            var tag = TestHelper.GetTag("hello_world.nbt", CompressionType.None);
            output.WriteLine(tag.Stringify(true));
        }

        [Fact]
        public void ParseSmall()
        {
            const string testString = "{name1:123,name2:\"sometext1\",name3:{subname1:456,subname2:\"sometext2\"}}";
            var lexer = new Lexer();
            foreach (var token in lexer.Tokenize(testString))
            {
                output.WriteLine($"{token.Type}: \"{token.Match.Trim()}\"");
            }
        }

        [Fact]
        public void ParseBig()
        {
            var testString = File.ReadAllText("/code/ruby/craftbook-nbt/test/bigtest.snbt");
            var lexer = new Lexer();
            foreach (var token in lexer.Tokenize(testString))
            {
                output.WriteLine($"{token.Type}: \"{token.Match.Trim()}\"");
            }
        }
    }
}