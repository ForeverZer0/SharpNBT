using System;
using System.IO;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Provides static methods for reading/writing the VarInt implementation commonly used by Minecraft to/from streams and buffers.
    /// </summary>
    /// <remarks>This is not part of the NBT API, and is merely included for convenience.</remarks>
    [PublicAPI]
    public class VarInt
    {
        private const int INT_SIZE = sizeof(int);

        /// <summary>
        /// Reads a VarInt from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> object to read from.</param>
        /// <returns>The VarInt value as a 32-bit integer.</returns>
        public static int Read([NotNull] Stream stream) => Read(stream, out var dummy);
        
        /// <summary>
        /// Reads a VarInt from the given <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">A buffer containing the VarInt bytes.</param>
        /// <returns>The VarInt value as a 32-bit integer.</returns>
        public static int Read(ReadOnlySpan<byte> buffer) => Read(buffer, out var dummy);

        /// <summary>
        /// Reads a VarInt from the given <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">A buffer containing the VarInt bytes.</param>
        /// <param name="start">The start index of the <paramref name="buffer"/> to begin reading at.</param>
        /// <param name="length">The maximum number of bytes to read from the <paramref name="buffer"/>.</param>
        /// <returns>The VarInt value as a 32-bit integer.</returns>
        public static int Read([NotNull] byte[] buffer, int start, int length) => Read(new ReadOnlySpan<byte>(buffer, start, length), out var dummy);
        
        /// <summary>
        /// Reads a VarInt from the given <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">A buffer containing the VarInt bytes.</param>
        /// <param name="start">The start index of the <paramref name="buffer"/> to begin reading at.</param>
        /// <param name="length">The maximum number of bytes to read from the <paramref name="buffer"/>.</param>
        /// <param name="count">A variable to store the number of bytes read from the <paramref name="buffer"/>.</param>
        /// <returns>The VarInt value as a 32-bit integer.</returns>
        public static int Read([NotNull] byte[] buffer, int start, int length, out int count) => Read(new ReadOnlySpan<byte>(buffer, start, length), out count);
        
        /// <summary>
        /// Reads a VarInt from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> object to read from.</param>
        /// <param name="count">A variable to store the number of bytes read from the <paramref name="stream"/>.</param>
        /// <returns>The VarInt value as a 32-bit integer.</returns>
        public static int Read([NotNull] Stream stream, out int count)
        {
            uint value = 0;
            count = 0;

            while (true)
            {
                if (count == INT_SIZE)
                    throw new OverflowException($"A VarInt cannot exceed {INT_SIZE} bytes.");

                var currentByte = (byte)stream.ReadByte();
                value |= unchecked((uint) (currentByte & 0x7F) << (count * 7));
                if ((currentByte & 0x80) == 0)
                    break;
                count++;
            }

            return unchecked((int) value);
        }
        
        /// <summary>
        /// Reads a VarInt from the given <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">A buffer containing the VarInt bytes.</param>
        /// <param name="count">A variable to store the number of bytes read from the <paramref name="buffer"/>.</param>
        /// <returns>The VarInt value as a 32-bit integer.</returns>
        public static int Read(ReadOnlySpan<byte> buffer, out int count)
        {
            uint value = 0;
            count = 0;

            while (true)
            {
                if (count == INT_SIZE)
                    throw new OverflowException($"A VarInt cannot exceed {INT_SIZE} bytes.");
                if (count >= buffer.Length)
                    throw new ArgumentException("Not enough data provided in buffer.", nameof(buffer));

                var currentByte = buffer[count];
                value |= unchecked((uint) (currentByte & 0x7F) << (count * 7));
                if ((currentByte & 0x80) == 0)
                    break;
                count++;
            }

            return unchecked((int) value);
        }
        
        /// <summary>
        /// Writes the given <paramref name="value"/> to the <paramref name="stream"/> as a VarInt.
        /// </summary>
        ///  <param name="stream">The <see cref="Stream"/> object to write to.</param>
        /// <param name="value">The value to be written.</param>
        /// <returns>The number of bytes that were written to the <paramref name="stream"/>.</returns>
        /// <exception cref="OverflowException">Thrown when the <paramref name="value"/> is too large for a VarInt.</exception>
        public static int Write([NotNull] Stream stream, int value)
        {
            Span<byte> buffer = stackalloc byte[INT_SIZE];
            var count = 0;

            var unsigned = unchecked((uint)value);
            while (true)
            {
                if (count == INT_SIZE)
                    throw new OverflowException("Int32 value exceeds the size of a VarInt.");
                
                var currentByte = (byte) (unsigned & 0x7F);
                unsigned >>= 7;
                if (unsigned != 0) 
                    currentByte |= 0x80;
                buffer[count] = currentByte;
                
                if (unsigned == 0)
                    break;
                count++;
            }
            
            stream.Write(buffer[..count]);
            return count;
        }

        /// <summary>
        /// Writes the given <paramref name="value"/> to the <paramref name="buffer"/> as a VarInt.
        /// </summary>
        ///  <param name="buffer">A buffer to write to..</param>
        /// <param name="value">The value to be written.</param>
        /// <returns>The number of bytes that were written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="OverflowException">Thrown when the <paramref name="value"/> is too large for a VarInt.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="buffer"/> is not large enough to contain the data.</exception>
        public static int Write(Span<byte> buffer, int value)
        {
            var count = 0;
            var unsigned = unchecked((uint)value);
            while (true)
            {
                if (count == INT_SIZE)
                    throw new OverflowException("Int32 value exceeds the size of a VarInt.");
                if (count >= buffer.Length)
                    throw new ArgumentException("Buffer is not large enough to contain data.", nameof(buffer));

                var currentByte = (byte) (unsigned & 0x7F);
                unsigned >>= 7;
                if (unsigned != 0) 
                    currentByte |= 0x80;
                buffer[count] = currentByte;
                
                if (unsigned == 0)
                    break;
                count++;
            }
            
            return count;
        }

        /// <summary>
        /// Writes the given <paramref name="value"/> to the <paramref name="buffer"/> as a VarInt.
        /// </summary>
        ///  <param name="buffer">A buffer to write to..</param>
        /// <param name="start">The start index of the <paramref name="buffer"/> to begin writing to.</param>
        /// <param name="length">The maximum number of bytes to write to the <paramref name="buffer"/>.</param>
        /// <param name="value">The value to be written.</param>
        /// <returns>The number of bytes that were written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="OverflowException">Thrown when the <paramref name="value"/> is too large for a VarInt.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="buffer"/> is not large enough to contain the data.</exception>
        public static int Write([NotNull] byte[] buffer, int start, int length, int value) => Write(new Span<byte>(buffer, start, length), value);
    }
}