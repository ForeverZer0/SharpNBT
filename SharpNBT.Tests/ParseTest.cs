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

        private static Stream GetFile(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream($"SharpNBT.Tests.Data.{filename}");
        }

        [Fact]
        public void ReadUncompressed()
        {
            using var stream = GetFile("hello_world.nbt");
            using var reader = new TagReader(stream, FormatOptions.Java);
            var compound = reader.ReadTag<CompoundTag>();
            output.WriteLine(compound.PrettyPrinted());
        }
        
        [Fact]
        public void ReadGZipped()
        {
            using var stream = GetFile("bigtest.nbt");
            using var gzip = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new TagReader(gzip, FormatOptions.Java);
            var compound = reader.ReadTag<CompoundTag>();
            output.WriteLine(compound.PrettyPrinted());
        }
        
    }
}