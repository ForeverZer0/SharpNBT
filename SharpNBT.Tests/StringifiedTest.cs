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
    }
}