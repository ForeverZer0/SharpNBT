using System.IO;
using System.IO.Compression;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class ReadWriteTest
    {
        private readonly ITestOutputHelper output;
        
        public ReadWriteTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ReadUncompressed()
        {
            var compound = TestHelper.GetTag("hello_world.nbt", CompressionType.None);
            output.WriteLine(compound.PrettyPrinted());
        }
        
        [Fact]
        public void ReadGZipped()
        {
            var compound = TestHelper.GetTag("bigtest.nbt", CompressionType.GZip);
            output.WriteLine(compound.PrettyPrinted());
        }
        
    }
}