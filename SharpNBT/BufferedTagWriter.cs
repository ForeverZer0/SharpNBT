using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Provides a <see cref="TagWriter"/> object that writes to an internal buffer instead of a <see cref="Stream"/> object, which then can be retrieved as
/// an array of bytes or written directly to a stream. This is especially convenient when creating packets to be sent over a network, where the size of
/// the packet must be pre-determined before sending.
/// </summary>
[PublicAPI]
public class BufferedTagWriter : TagWriter
{
    private readonly MemoryStream buffer;

    /// <summary>
    /// Creates a new instance of the <see cref="BufferedTagWriter"/> class.
    /// </summary>
    /// <param name="compression">Indicates the compression algorithm used to compress the file.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <returns>A newly created <see cref="BufferedTagWriter"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid compression type is specified.</exception>
    public static BufferedTagWriter Create(CompressionType compression, FormatOptions options)
    {
        // ReSharper disable once IntroduceOptionalParameters.Global
        return Create(compression, options, 4096);
    }
        
    /// <summary>
    /// Creates a new instance of the <see cref="BufferedTagWriter"/> class.
    /// </summary>
    /// <param name="compression">Indicates the compression algorithm used to compress the file.</param>
    /// <param name="options">Bitwise flags to configure how data should be handled for compatibility between different specifications.</param>
    /// <param name="capacity">The initial capacity of the buffer.</param>
    /// <returns>A newly created <see cref="BufferedTagWriter"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid compression type is specified.</exception>
    public static BufferedTagWriter Create(CompressionType compression, FormatOptions options, int capacity)
    {
        var buffer = new MemoryStream(capacity);
        Stream stream = compression switch
        {
            CompressionType.None => buffer,
            CompressionType.GZip => new GZipStream(buffer, CompressionMode.Compress, false),
            CompressionType.ZLib => new ZLibStream(buffer, CompressionMode.Compress),
            _ => throw new ArgumentOutOfRangeException(nameof(compression), compression, null)
        };

        return new BufferedTagWriter(stream, buffer, options);
    }
        
    private BufferedTagWriter(Stream stream, MemoryStream buffer, FormatOptions options) : base(stream, options, false)
    {
        this.buffer = buffer;
    }

    /// <summary>
    /// Gets the number of bytes in the internal buffer.
    /// </summary>
    public long Length
    {
        get
        {
            BaseStream.Flush();
            return buffer.Length;
        }
    }

    /// <summary>
    /// Gets the capacity of the internal buffer.
    /// </summary>
    /// <remarks>The capacity will expand automatically as-needed.</remarks>
    public long Capacity => buffer.Capacity;

    /// <summary>
    /// Gets the internal buffer as an array of bytes containing the NBT data written so far.
    /// </summary>
    /// <returns>An array of bytes containing the NBT data.</returns>
    [Pure]
    public byte[] ToArray()
    {
        BaseStream.Flush();
        return buffer.ToArray();
    }

    /// <summary>
    /// Copies the internal buffer to the specified <paramref name="stream"/>;
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> instance to write to.</param>
    public void CopyTo(Stream stream)
    {
        BaseStream.Flush();
        buffer.CopyTo(stream);
    }

    /// <summary>
    /// Asynchronously copies the internal buffer to the specified <paramref name="stream"/>;
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> instance to write to.</param>
    public async Task CopyToAsync(Stream stream)
    {
        await BaseStream.FlushAsync();
        await buffer.CopyToAsync(stream);
    }

    /// <summary>
    /// Implicit conversion of <see cref="BufferedTagWriter"/> to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static implicit operator ReadOnlySpan<byte>(BufferedTagWriter writer)
    {
        writer.BaseStream.Flush();
        return writer.buffer.ToArray();
    }
}