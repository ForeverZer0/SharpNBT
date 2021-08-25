using System;
using JetBrains.Annotations;

namespace SharpNBT
{
    /// <summary>
    /// Describes the specification to use for reading/writing.
    /// </summary>
    /// <remarks>
    /// There are some major changes between the original Java version, and the Bedrock editions of Minecraft that makes them incompatible with one another.
    /// Furthermore, the Bedrock editions use a different specification depending on whether it is writing to disk or sending over a network.
    /// </remarks>
    /// <seealso href="https://wiki.vg/NBT#Bedrock_edition"/>
    [PublicAPI][Flags]
    public enum FormatOptions
    {
        /// <summary>
        /// None/invalid option flags.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Numeric values will be read/written in big-endian format.
        /// </summary>
        /// <remarks>This is the default for the Java edition of Minecraft.</remarks>
        BigEndian = 0x01,
        
        /// <summary>
        /// Numeric values will be read/written in little-endian format.
        /// </summary>
        /// <remarks>This is the default for Bedrock editions of Minecraft.</remarks>
        LittleEndian = 0x02,
        
        /// <summary>
        /// Integer types will be read/written as variable-length integers.
        /// </summary>
        VarIntegers = 0x04,
        
        /// <summary>
        /// Variable-length integers will be written using ZigZag encoding.
        /// </summary>
        /// <see href="http://neurocline.github.io/dev/2015/09/17/zig-zag-encoding.html"/>
        ZigZagEncoding = 0x08,
        
        /// <summary>
        /// Flags for using a format compatible with Java editions of Minecraft.
        /// </summary>
        Java = BigEndian,
        
        /// <summary>
        /// Flags for using a format compatible with Bedrock editions of Minecraft when writing to a file.
        /// </summary>
        BedrockFile = LittleEndian,
        
        /// <summary>
        /// Flags for using a format compatible with Bedrock editions of Minecraft when transporting across a network..
        /// </summary>
        BedrockNetwork = LittleEndian | VarIntegers | ZigZagEncoding
    }
}