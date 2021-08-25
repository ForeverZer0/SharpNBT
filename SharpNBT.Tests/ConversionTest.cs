using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class ConversionTest
    {
        private readonly ITestOutputHelper output;
        
        public ConversionTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        public void JsonOutput1()
        {
            var bigtest = TestHelper.GetTag("bigtest.nbt", CompressionType.GZip);
            output.WriteLine(bigtest.ToJsonString(true));
        }
    }
}