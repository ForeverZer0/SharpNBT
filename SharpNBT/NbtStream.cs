using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Provides methods and properties to read and write as a stream using the NBT format specification.
    /// </summary>
    [PublicAPI]
    public partial class NbtStream : Stream
    {
        protected readonly Stream BaseStream;
        private readonly Stack<Tag> topLevel;
        
        // /// <summary>
        // /// Opens a <see cref="NbtStream"/> on the specified <paramref name="path"/> with read/write access.
        // /// </summary>
        // /// <param name="path">The path to a file to open.</param>
        // /// <param name="mode">
        // /// A value that specified whether a file is created if one does not exist, and determines
        // /// whether the contents of an existing file are retained or overwritten.
        // /// </param>
        // /// <returns>A <see cref="NbtStream"/> opened in the specified mode and path, with read/write access.</returns>
        // public static NbtStream Open(string path, FileMode mode) => new(File.Open(path, mode));
        //
        // public static NbtStream Open(string path, FileMode mode, FileAccess access) => new(File.Open(path, mode, access));

        public static NbtStream OpenRead(string path)
        {
            var compressed = false;
            
            // Look for GZIP magic number
            using (var str = File.OpenRead(path))
            {
                if (str.ReadByte() == 0x1F && str.ReadByte() == 0x8B)
                    compressed = true;
            }

            return compressed ? new NbtStream(File.OpenRead(path), CompressionMode.Decompress) : new NbtStream(File.OpenRead(path));
        }

        public static NbtStream OpenWrite(string path, CompressionLevel level = CompressionLevel.NoCompression)
        {
            if (level != CompressionLevel.NoCompression)
            {
                var stream = new GZipStream(File.OpenWrite(path), level, false);
                return new NbtStream(stream);
            }
            return new NbtStream(File.OpenWrite(path));
        }
        
        public NbtStream(Stream stream, bool leaveOpen = false)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            topLevel = new Stack<Tag>();
        }
        
        public NbtStream(Stream stream, CompressionMode compression, bool leaveOpen = false) : this(new GZipStream(stream, compression, leaveOpen), leaveOpen)
        {
        }

        public NbtStream([NotNull] byte[] buffer) : this(new MemoryStream(buffer), false)
        {
        }

        /// <summary>Clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.Flush?view=netcore-5.0">`Stream.Flush` on docs.microsoft.com</a></footer>
        public override void Flush() => BaseStream.Flush();

        /// <summary>Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.Read?view=netcore-5.0">`Stream.Read` on docs.microsoft.com</a></footer>
        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);

        /// <summary>Sets the position within the current stream.</summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <returns>The new position within the current stream.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.Seek?view=netcore-5.0">`Stream.Seek` on docs.microsoft.com</a></footer>
        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        /// <summary>Sets the length of the current stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.SetLength?view=netcore-5.0">`Stream.SetLength` on docs.microsoft.com</a></footer>
        public override void SetLength(long value) => BaseStream.SetLength(value);

        /// <summary>Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred, such as the specified file cannot be found.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// <see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)" /> was called after the stream was closed.</exception>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.Write?view=netcore-5.0">`Stream.Write` on docs.microsoft.com</a></footer>
        public override void Write(byte[] buffer, int offset, int count) =>  BaseStream.Write(buffer, offset, count);

        /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
        /// <returns>
        /// <see langword="true" /> if the stream supports reading; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.CanRead?view=netcore-5.0">`Stream.CanRead` on docs.microsoft.com</a></footer>
        public override bool CanRead => BaseStream.CanRead;

        /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
        /// <returns>
        /// <see langword="true" /> if the stream supports seeking; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.CanSeek?view=netcore-5.0">`Stream.CanSeek` on docs.microsoft.com</a></footer>
        public override bool CanSeek => BaseStream.CanSeek;

        /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
        /// <returns>
        /// <see langword="true" /> if the stream supports writing; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.CanWrite?view=netcore-5.0">`Stream.CanWrite` on docs.microsoft.com</a></footer>
        public override bool CanWrite => BaseStream.CanWrite;

        /// <summary>Gets the length in bytes of the stream.</summary>
        /// <exception cref="T:System.NotSupportedException">A class derived from <see langword="Stream" /> does not support seeking.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.Length?view=netcore-5.0">`Stream.Length` on docs.microsoft.com</a></footer>
        public override long Length => BaseStream.Length;

        /// <summary>Gets or sets the position within the current stream.</summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <returns>The current position within the stream.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.Stream.Position?view=netcore-5.0">`Stream.Position` on docs.microsoft.com</a></footer>
        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }
        
    }
}