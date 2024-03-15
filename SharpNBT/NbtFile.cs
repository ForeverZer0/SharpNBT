using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Provides static convenience methods for working with NBT-formatted files, including both reading and writing.
/// </summary>
[PublicAPI]
public static class NbtFile
{

    /// <summary>
    /// Reads a file at the given <paramref name="path"/> and deserializes the top-level <see cref="Tag"/> contained in the file.
    /// </summary>
    /// <param name="path">The path to the file to be read.</param>
    /// <param name="compression">Indicates the compression algorithm used to compress the file.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <typeparam name="T">The type of tag to deserialize.</typeparam>
    /// <returns>The deserialized <see cref="Tag"/> instance.</returns>
    public static T Read<T>(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect) where T : Tag, ICollection<Tag>
    {
        using var reader = new TagReader(GetReadStream(path, compression), options);
        return reader.ReadTag<T>();
    }
        
    /// <summary>
    /// Reads a file at the given <paramref name="path"/> and deserializes the top-level <see cref="CompoundTag"/> contained in the file.
    /// </summary>
    /// <param name="path">The path to the file to be read.</param>
    /// <param name="compression">Indicates the compression algorithm used to compress the file.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
    public static CompoundTag Read(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect)
    {
        using var reader = new TagReader(GetReadStream(path, compression), options);
        return reader.ReadTag<CompoundTag>();
    }

    /// <summary>
    /// Asynchronously reads a file at the given <paramref name="path"/> and deserializes the top-level <see cref="Tag"/> contained in the file.
    /// </summary>
    /// <param name="path">The path to the file to be read.</param>
    /// <param name="compression">Indicates the compression algorithm used to compress the file.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <returns>The deserialized <see cref="Tag"/> instance.</returns>
    public static async Task<T> ReadAsync<T>(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect) where T : Tag, ICollection<Tag>
    {
        await using var reader = new TagReader(GetReadStream(path, compression), options);
        return await reader.ReadTagAsync<T>();
    }
        
    /// <summary>
    /// Asynchronously reads a file at the given <paramref name="path"/> and deserializes the top-level <see cref="CompoundTag"/> contained in the file.
    /// </summary>
    /// <param name="path">The path to the file to be read.</param>
    /// <param name="compression">Indicates the compression algorithm used to compress the file.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <returns>The deserialized <see cref="CompoundTag"/> instance.</returns>
    public static async Task<CompoundTag> ReadAsync(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect)
    {
        await using var reader = new TagReader(GetReadStream(path, compression), options);
        return await reader.ReadTagAsync<CompoundTag>();
    }

    /// <summary>
    /// Writes the given <paramref name="tag"/> to a file at the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the file to be written to.</param>
    /// <param name="tag">The top-level <see cref="CompoundTag"/> instance to be serialized.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <param name="type">A flag indicating the type of compression to use.</param>
    /// <param name="level">Indicates a compression strategy to be used, if any.</param>
    public static void Write(string path, CompoundTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
    {
        using var stream = File.OpenWrite(path);
        using var writer = new TagWriter(GetWriteStream(stream, type, level), options);
        writer.WriteTag(tag);
    }
        
    /// <inheritdoc cref="Write(string,SharpNBT.CompoundTag,SharpNBT.FormatOptions,SharpNBT.CompressionType,System.IO.Compression.CompressionLevel)"/>
    public static void Write(string path, ListTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
    {
        using var stream = File.OpenWrite(path);
        using var writer = new TagWriter(GetWriteStream(stream, type, level), options);
        writer.WriteTag(tag);
    }
        
    /// <summary>
    /// Asynchronously writes the given <paramref name="tag"/> to a file at the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the file to be written to.</param>
    /// <param name="tag">The top-level <see cref="CompoundTag"/> instance to be serialized.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <param name="type">A flag indicating the type of compression to use.</param>
    /// <param name="level">Indicates a compression strategy to be used, if any.</param>
    public static async Task WriteAsync(string path, CompoundTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
    {
        await using var stream = File.OpenWrite(path);
        await using var writer = new TagWriter(GetWriteStream(stream, type, level), options);
        await writer.WriteTagAsync(tag);
    }
        
    /// <inheritdoc cref="WriteAsync(string,SharpNBT.CompoundTag,SharpNBT.FormatOptions,SharpNBT.CompressionType,System.IO.Compression.CompressionLevel)"/>
    public static async Task WriteAsync(string path, ListTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
    {
        await using var stream = File.OpenWrite(path);
        await using var writer = new TagWriter(GetWriteStream(stream, type, level), options);
        await writer.WriteTagAsync(tag);
    }

    /// <summary>
    /// Opens an existing NBT file for reading, and returns a <see cref="TagReader"/> instance for it.
    /// </summary>
    /// <param name="path">The path of the file to query write.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <param name="compression">Indicates the compression algorithm used to compress the file.</param>
    /// <returns>A <see cref="TagReader"/> instance for the file stream.</returns>
    /// <remarks>File compression will be automatically detected and used handled when necessary.</remarks>
    public static TagReader OpenRead(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect)
    {
        return new TagReader(GetReadStream(path, compression), options);
    }

    /// <summary>
    /// Opens an existing NBT file or creates a new one if one if it does not exist, and returns a <see cref="TagWriter"/> instance for it.
    /// </summary>
    /// <param name="path">The path of the file to query write.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <param name="type">A flag indicating the type of compression to use.</param>
    /// <param name="level">A flag indicating the compression strategy that will be used, if any.</param>
    /// <returns>A <see cref="TagWriter"/> instance for the file stream.</returns>
    public static TagWriter OpenWrite(string path, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
    {
        var stream = GetWriteStream(File.OpenWrite(path), type, level);
        return new TagWriter(stream, options);
    }

    private static Stream GetWriteStream(Stream stream, CompressionType type, CompressionLevel level)
    {
        switch (type)
        {
            case CompressionType.None: return stream;
            case CompressionType.GZip: return new GZipStream(stream, level, false);
            case CompressionType.ZLib: return new ZLibStream(stream, level);
            case CompressionType.AutoDetect:
                throw new ArgumentOutOfRangeException(nameof(type), Strings.AutoDetectNotValid);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private static Stream GetReadStream(string path, CompressionType compression)
    {
        var stream = File.OpenRead(path);
        if (compression == CompressionType.AutoDetect)
        {
            var firstByte = (byte) stream.ReadByte();
            stream.Seek(0, SeekOrigin.Begin);

            compression = firstByte switch
            {
                0x78 => CompressionType.ZLib,
                0x1F => CompressionType.GZip,
                0x08 => CompressionType.None, // ListTag (valid in Bedrock)
                0x0A => CompressionType.None, // CompoundTag
                _ => throw new FormatException(Strings.CannotDetectCompression)
            };
        }

        return compression switch
        {
            CompressionType.None => stream,
            CompressionType.GZip => new BufferedStream(new GZipStream(stream, CompressionMode.Decompress, false)),
            CompressionType.ZLib => new BufferedStream(new ZLibStream(stream, CompressionMode.Decompress)),
            _ => throw new ArgumentOutOfRangeException(nameof(compression), compression, null)
        };
    }
        
}