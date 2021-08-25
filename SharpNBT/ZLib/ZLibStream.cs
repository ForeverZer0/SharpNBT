using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT.ZLib
{
    /// <summary>
    /// ZLib stream implementation for reading/writing.
    /// <para/>
    /// Generally speaking, the ZLib format is merely a DEFLATE stream, prefixed with a header, and performs a cyclic redundancy check to ensure data integrity
    /// by storing an Adler-32 checksum after the compressed payload.
    /// </summary>
    [PublicAPI]
    public class ZLibStream : Stream
    {
        protected readonly DeflateStream DeflateStream;
        protected readonly Stream BaseStream;
        
        private readonly CompressionMode compressionMode;
        private readonly bool leaveOpen;
        private readonly Adler32 adler32 = new Adler32();
        private bool isClosed;
        private byte[] checksum;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsSupported"/> class using the specified compression <paramref name="level"/> and <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance to be compressed.</param>
        /// <param name="level">The level of compression to use.</param>
        public ZLibStream([NotNull] Stream stream, CompressionLevel level) : this(stream, level, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsSupported"/> class using the specified compression <paramref name="mode"/> and <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance to be compressed or uncompressed.</param>
        /// <param name="mode">The type of compression to use.</param>
        public ZLibStream([NotNull] Stream stream, CompressionMode mode) : this(stream, mode, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZLibStream"/> class using the specified compression <paramref name="level"/> and <paramref name="stream"/>,
        /// and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance to be compressed.</param>
        /// <param name="level">The level of compression to use.</param>
        /// <param name="leaveOpen">Indicates if the <paramref name="stream"/> should be left open after this <see cref="ZLibStream"/> is closed.</param>
        public ZLibStream([NotNull] Stream stream, CompressionLevel level, bool leaveOpen)
        {
            compressionMode = CompressionMode.Compress;
            this.leaveOpen = leaveOpen;
            BaseStream = stream;
            DeflateStream = CreateStream(level);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZLibStream"/> class using the specified compression <paramref name="mode"/> and <paramref name="stream"/>,
        /// and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance to be compressed or uncompressed.</param>
        /// <param name="mode">The type of compression to use.</param>
        /// <param name="leaveOpen">Indicates if the <paramref name="stream"/> should be left open after this <see cref="ZLibStream"/> is closed.</param>
        public ZLibStream([NotNull] Stream stream, CompressionMode mode, bool leaveOpen)
        {
            compressionMode = mode;
            this.leaveOpen = leaveOpen;
            BaseStream = stream;
            DeflateStream = CreateStream(CompressionLevel.Fastest);
        }

        /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
        /// <returns>
        /// <see langword="true" /> if the stream supports reading; otherwise, <see langword="false" />.</returns>
        public override bool CanRead => compressionMode == CompressionMode.Decompress && !isClosed;

        /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
        /// <returns>
        /// <see langword="true" /> if the stream supports writing; otherwise, <see langword="false" />.</returns>
        public override bool CanWrite => compressionMode == CompressionMode.Compress && !isClosed;

        /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
        /// <returns>
        /// <see langword="true" /> if the stream supports seeking; otherwise, <see langword="false" />.</returns>
        public override bool CanSeek => false;

        /// <summary>Gets the length in bytes of the stream.</summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="NotSupportedException">This property is not supported and will always throw an exception.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Length => throw new NotSupportedException("Stream does not support this function.");

        /// <summary>Gets or sets the position within the current stream.</summary>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">This property is not supported and will always throw an exception.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Position
        {
            get => throw new NotSupportedException("Stream does not support getting/setting position.");
            set => throw new NotSupportedException("Stream does not support getting/setting position.");
        }

        /// <summary>Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.</summary>
        /// <returns>The unsigned byte cast to an <see langword="Int32" />, or -1 if at the end of the stream.</returns>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int ReadByte()
        {
            var n = DeflateStream.ReadByte();
            if (n == -1)  // EOF
                ReadCrc();
            else
                adler32.Update(Convert.ToByte(n));

            return n;
        }

        /// <summary>Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced by the bytes read from the current source.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes allocated in the buffer if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(Span<byte> buffer)
        {
            var read = DeflateStream.Read(buffer);
            if (read < 1 && buffer.Length > 0)
                ReadCrc();
            else
                adler32.Update(buffer[..read]);
            
            return read;
        }

        /// <summary>Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

        /// <summary>Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.</summary>
        /// <param name="buffer">The region of memory to write the data into.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of its <see cref="P:System.Threading.Tasks.ValueTask`1.Result" /> property contains the total number of bytes read into the buffer. The result value can be less than the number of bytes allocated in the buffer if that many bytes are not currently available, or it can be 0 (zero) if the end of the stream has been reached.</returns>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var read = await DeflateStream.ReadAsync(buffer, cancellationToken);
            adler32.Update(buffer.Slice(0, read).Span);
            return read;
        }

        /// <summary>Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.</summary>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data from the stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of the task parameter contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await DeflateStream.ReadAsync(buffer, offset, count, cancellationToken);
            adler32.Update(new ReadOnlySpan<byte>(buffer, offset, read));
            return read;
        }

        /// <summary>Writes a byte to the current position in the stream and advances the position within the stream by one byte.</summary>
        /// <param name="value">The byte to write to the stream.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing, or the stream is already closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void WriteByte(byte value)
        {
            DeflateStream.WriteByte(value);
            adler32.Update(value);
        }

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
        public override void Write(byte[] buffer, int offset, int count)
        {
            DeflateStream.Write(buffer, offset, count);
            adler32.Update(buffer, offset, count);
        }

        /// <summary>Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
        /// <param name="buffer">A region of memory. This method copies the contents of this region to the current stream.</param>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            DeflateStream.Write(buffer);
            adler32.Update(buffer);
        }

        /// <summary>Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.</summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> from which to begin copying bytes to the stream.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous write operation.</exception>
        public override async Task WriteAsync([NotNull] byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await DeflateStream.WriteAsync(buffer, offset, count, cancellationToken);
            adler32.Update(new ReadOnlySpan<byte>(buffer, offset, count));
        }

        /// <summary>Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.</summary>
        /// <param name="buffer">The region of memory to write data from.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await DeflateStream.WriteAsync(buffer, cancellationToken);
            adler32.Update(buffer.Span);
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// Instead of calling this method, ensure that the stream is properly disposed.
        /// </summary>
        public override void Close()
        {
            if (isClosed)
                return;
            
            isClosed = true;
            if (compressionMode == CompressionMode.Compress)
            {
                Flush();
                DeflateStream.Close();

                checksum = BitConverter.GetBytes(adler32.Value);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(checksum);
                BaseStream.Write(checksum, 0, checksum.Length);
            }
            else
            {
                DeflateStream.Close();
                if (checksum == null)
                    ReadCrc();
            }

            if (!leaveOpen)
                BaseStream.Close();
        }

        /// <summary>Clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        public override void Flush()
        {
            DeflateStream?.Flush();
            BaseStream?.Flush();
        }

        /// <summary>Sets the position within the current stream.</summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <summary>Sets the length of the current stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <summary>
        /// Checks if the given <paramref name="stream"/> is in ZLib format.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance to query.</param>
        /// <returns><see langword="true"/> is <paramref name="stream"/> is in a supported ZLib format, otherwise <see langword="false"/> if not or an error occured.</returns>
        public static bool IsSupported(Stream stream)
        {
            int cmf;
            int flag;

            if (!stream.CanRead) 
                return false;
            
            if (stream.Position != 0)
            {
                var pos = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                cmf = stream.ReadByte();
                flag = stream.ReadByte();
                stream.Seek(pos, SeekOrigin.Begin);
            }
            else
            {
                cmf = stream.ReadByte();
                flag = stream.ReadByte();
            }

            try
            {
                var header = ZLibHeader.Decode(cmf, flag);
                return header.IsSupported;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads the last 4 bytes of the stream where the CRC is stored.
        /// </summary>
        /// <exception cref="EndOfStreamException">Thrown when the stream is cannot seek to the checksum location to read.</exception>
        /// <exception cref="InvalidDataException">Thrown when the checksum comparison does not match.</exception>
        private void ReadCrc()
        {
            checksum = new byte[4];
            BaseStream.Seek(-4, SeekOrigin.End);
            if (BaseStream.Read(checksum, 0, 4) < 4)
                throw new EndOfStreamException();

            if (BitConverter.IsLittleEndian)
                Array.Reverse(checksum);

            var crcAdler = adler32.Value;
            var crcStream = BitConverter.ToInt32(checksum, 0);

            if (crcStream != crcAdler)
                throw new InvalidDataException(Strings.CRCFail);
        }

        /// <summary>
        /// Initializes the underlying <see cref="System.IO.Compression.DeflateStream"/> instance.
        /// </summary>
        private DeflateStream CreateStream(CompressionLevel compressionLevel)
        {
            switch (compressionMode)
            {
                case CompressionMode.Compress:
                {
                    WriteHeader(compressionLevel);
                    return new DeflateStream(BaseStream, compressionLevel, true);
                }
                case CompressionMode.Decompress:
                {
                    if (!IsSupported(BaseStream))
                        throw new InvalidDataException(Strings.ZlibUnsupported);

                    return new DeflateStream(BaseStream, CompressionMode.Decompress, true);
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(compressionMode));
            }
        }

        /// <summary>
        /// Writes the ZLib header to the stream.
        /// </summary>
        /// <param name="compressionLevel">The compression level being used.</param>
        protected void WriteHeader(CompressionLevel compressionLevel)
        {
            var header = new ZLibHeader(compressionLevel);
            var magicNumber = header.Encode();
            BaseStream.WriteByte(magicNumber[0]);
            BaseStream.WriteByte(magicNumber[1]);
        }
    }
}
