using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Abstract base class for the <see cref="TagReader"/> and <see cref="TagWriter"/> classes, providing shared functionality.
    /// </summary>
    [PublicAPI]
    public abstract class TagIO : IDisposable
    {
        /// <summary>
        /// Gets the underlying stream this instance is operating on.
        /// </summary>
        [NotNull]
        protected Stream BaseStream { get; }
        
        /// <summary>
        /// Gets a flag indicating if byte swapping is required for numeric values, accounting for both the endianness of the host machine and the
        /// specified <see cref="FormatOptions"/>.
        /// </summary>
        protected bool SwapEndian { get; }
        
        /// <summary>
        /// Gets a flag indicating if variable-length integers should be used in applicable places.
        /// </summary>
        protected bool UseVarInt { get; }
        
        /// <summary>
        /// Gets a flag indicating if variable-length integers will be "ZigZag encoded".
        /// </summary>
        /// <see href="https://developers.google.com/protocol-buffers/docs/encoding#varints"/>
        public bool ZigZagEncoding { get; }
        
        /// <summary>
        /// Gets the format to be followed for compatibility.
        /// </summary>
        public FormatOptions FormatOptions { get; }

        protected TagIO([NotNull] Stream stream,  FormatOptions options)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            
            if (options.HasFlag(FormatOptions.BigEndian))
                SwapEndian = BitConverter.IsLittleEndian;
            else if (options.HasFlag(FormatOptions.LittleEndian))
                SwapEndian = !BitConverter.IsLittleEndian;
            
            UseVarInt = options.HasFlag(FormatOptions.VarIntegers);
            ZigZagEncoding = options.HasFlag(FormatOptions.ZigZagEncoding);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Asynchronously releases the unmanaged resources used by the <see cref="TagIO"/> instance.
        /// </summary>
        public abstract ValueTask DisposeAsync();

    }
}