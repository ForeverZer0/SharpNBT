using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using SharpNBT.ZLib;

namespace SharpNBT.Tests
{
    public class TestHelper
    {
        public static Stream GetFile(string filename, CompressionType compression)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream($"SharpNBT.Tests.Data.{filename}");
            if (stream is null)
                throw new FileNotFoundException(filename);
            
            return compression switch
            {
                CompressionType.None => stream,
                CompressionType.GZip => new GZipStream(stream, CompressionMode.Decompress),
                CompressionType.ZLib => new ZLibStream(stream, CompressionMode.Decompress),
                _ => throw new Exception()
            };
        }

        public static CompoundTag GetTag(string filename, CompressionType compression)
        {
            using var stream = GetFile(filename, compression);
            using var reader = new TagReader(stream, FormatOptions.Java);
            return reader.ReadTag<CompoundTag>();
        }
    }
}