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
        None = 0,
        
        BigEndian = 0x01,
        
        LittleEndian = 0x02,
        
        VarIntegers = 0x04,
        
        ZigZagEncoding = 0x08,
        
        
        Java = BigEndian,
        
        BedrockFile = LittleEndian,
        
        BedrockNetwork = LittleEndian | VarIntegers | ZigZagEncoding
    }
}