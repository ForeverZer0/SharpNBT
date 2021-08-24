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
    [PublicAPI]
    public enum Specification
    {
        /// <summary>
        /// The original NBT specification which encodes all numbers in big-endian format.
        /// </summary>
        /// <remarks>This is the specification used by Java editions of Minecraft.</remarks>
        Standard,
        
        /// <summary>
        /// Similar to <see cref="Standard"/>, but numbers are in little-endian format.
        /// </summary>
        /// <remarks>This specification is used by Bedrock editions of Minecraft when reading/writing to <b>disk</b>.</remarks>
        LittleEndian,
        
        /// <summary>
        /// Similar to <see cref="LittleEndian"/> format, but uses variable-length integers in place of many fixed-length integers.
        /// </summary>
        /// <remarks>This specification is used by Bedrock editions of Minecraft when reading/writing to a <b>network stream</b>.</remarks>
        VarInt
    }
}