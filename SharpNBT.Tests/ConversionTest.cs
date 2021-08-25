using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class ConversionTest
    {
        private readonly ITestOutputHelper output;
        private readonly CompoundTag tag;
        
        public ConversionTest(ITestOutputHelper output)
        {
            this.output = output;
            tag = TestHelper.GetTag("bigtest.nbt", CompressionType.GZip);
        }
        
        [Fact]
        public void JsonOutput1()
        {
            output.WriteLine(tag.ToJsonString(true));
        }
    }
}