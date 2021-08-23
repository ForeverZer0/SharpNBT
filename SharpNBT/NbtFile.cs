using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Provides static convenience methods for working with NBT-formatted files, including both reading and writing.
    /// </summary>
    [PublicAPI]
    public static class NbtFile
    {
        /// <summary>
        /// Reads a file at the given <paramref name="path"/> and deserializes the top-level <see cref="CompoundTag"/> contained in the file.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
        public static CompoundTag Read([NotNull] string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new TagReader(stream, IsCompressed(path), false);
            return reader.ReadTag<CompoundTag>();
        }
        
        /// <summary>
        /// Asynchronously reads a file at the given <paramref name="path"/> and deserializes the top-level <see cref="CompoundTag"/> contained in the file.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
        public static async Task<CompoundTag> ReadAsync([NotNull] string path)
        {
            await using var stream = File.OpenRead(path);
            await using var reader = new TagReader(stream, IsCompressed(path), false);
            return await reader.ReadTagAsync<CompoundTag>();
        }
        
        /// <summary>
        /// Writes the given <paramref name="tag"/> to a file at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the file to be written to.</param>
        /// <param name="tag">The top-level <see cref="CompoundTag"/> instance to be serialized.</param>
        /// <param name="compression">Indicates a compression strategy to be used, if any.</param>
        public static void Write([NotNull] string path, [NotNull] CompoundTag tag, CompressionLevel compression = CompressionLevel.NoCompression)
        {
            using var stream = File.OpenWrite(path);
            using var writer = new TagWriter(stream, compression);
            writer.WriteTag(tag);
        }
        
        public static async Task WriteAsync([NotNull] string path, [NotNull] CompoundTag tag, CompressionLevel compression = CompressionLevel.NoCompression)
        {
            await using var stream = File.OpenWrite(path);
            await using var writer = new TagWriter(stream, compression);
            await writer.WriteTagAsync(tag);
        }

        /// <summary>
        /// Detects the presence of a GZip compressed file at the given <paramref name="path"/> by searching for the "magic number" in the header.
        /// </summary>
        /// <param name="path">The path of the file to query.</param>
        /// <returns><see langword="true"/> if GZip compression was detected, otherwise <see langword="false"/>.</returns>
        public static bool IsCompressed([NotNull] string path)
        {
            using var str = File.OpenRead(path);
            return str.ReadByte() == 0x1F && str.ReadByte() == 0x8B;
        }

        /// <summary>
        /// Opens an existing NBT file for reading, and returns a <see cref="TagReader"/> instance for it.
        /// </summary>
        /// <param name="path">The path of the file to query write.</param>
        /// <returns>A <see cref="TagReader"/> instance for the file stream.</returns>
        /// <remarks>File compression will be automatically detected and used handled when necessary.</remarks>
        public static TagReader OpenRead([NotNull] string path)
        {
            var compressed = IsCompressed(path);
            var stream = File.OpenRead(path);
            return new TagReader(stream, compressed, false);
        }

        /// <summary>
        /// Opens an existing NBT file or creates a new one if one if it does not exist, and returns a <see cref="TagWriter"/> instance for it.
        /// </summary>
        /// <param name="path">The path of the file to query write.</param>
        /// <param name="compression">A flag indicating the compression strategy that will be used, if any.</param>
        /// <returns>A <see cref="TagWriter"/> instance for the file stream.</returns>
        public static TagWriter OpenWrite([NotNull] string path, CompressionLevel compression = CompressionLevel.NoCompression)
        {
            var stream = File.OpenWrite(path);
            if (compression == CompressionLevel.NoCompression)
                return new TagWriter(stream, compression);
            
            var gzip = new GZipStream(stream, compression, false);
            return new TagWriter(gzip, compression);
        }
    }
}