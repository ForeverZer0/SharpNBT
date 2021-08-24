using System;
using System.IO;
using System.IO.Compression;
using Xunit;

namespace SharpNBT.Tests
{
    public class CompressionTest
    {

        [Fact]
        public void WriteUncompressed()
        {
            var compound = new CompoundTag("Top-Level Compound");
            compound.Add(new ByteTag("Child Byte", 255));
            compound.Add(new StringTag("Child String", "Hello World!"));

            using var stream = NbtFile.OpenWrite("./Data/write-test-uncompressed.nbt", CompressionType.None);
            stream.WriteTag(compound);
        }
        
        [Fact]
        public void WriteCompressed()
        {
            var compound = new CompoundTag("Top-Level Compound");
            compound.Add(new ByteTag("Child Byte", 255));
            compound.Add(new StringTag("Child String", "Hello World!"));
        
            using var stream = NbtFile.OpenWrite("./Data/write-test-compressed.nbt", CompressionType.GZip, CompressionLevel.Optimal);
            stream.WriteTag(compound);
        }
        
        
        [Fact]
        public void ReadUncompressed()
        {
            using var stream = NbtFile.OpenRead("./Data/hello_world.nbt");
            var compound = stream.ReadTag<CompoundTag>();
            Assert.Equal("hello world", compound.Name);
        }

        [Fact]
        public void ReadCompressed()
        {
            using var stream = NbtFile.OpenRead("./Data/bigtest.nbt");
            var compound = stream.ReadTag<CompoundTag>();
            Assert.Equal("Level", compound.Name);
        }
    }
}