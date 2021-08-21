using System;
using Xunit;

namespace SharpNBT.Tests
{
    public class CompressionTest : IDisposable
    {
        public void Dispose()
        {
            
        }
        
        [Fact]
        public void ReadUncompressed()
        {
            using var stream = NbtStream.OpenRead("./Data/hello_world.nbt");
            var compound = stream.ReadTag<CompoundTag>();
            Assert.Equal("hello world", compound.Name);
        }

        [Fact]
        public void ReadCompressed()
        {
            using var stream = NbtStream.OpenRead("./Data/bigtest.nbt");
            var compound = stream.ReadTag<CompoundTag>();
            Assert.Equal("Level", compound.Name);
        }
    }
}