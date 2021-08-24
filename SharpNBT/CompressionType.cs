using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Describes compression formats supported by the NBT specification.
    /// </summary>
    [PublicAPI]
    public enum CompressionType : byte
    {
        /// <summary>
        /// No compression.
        /// </summary>
        None,
        
        /// <summary>
        /// GZip compression
        /// </summary>
        GZip,
        
        /// <summary>
        /// ZLib compression
        /// </summary>
        ZLib,
        
        /// <summary>
        /// Automatically detect compression using magic numbers.
        /// </summary>
        AutoDetect = 0xFF
    }

}