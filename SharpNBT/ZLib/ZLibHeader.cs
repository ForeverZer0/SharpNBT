using System;
using System.IO.Compression;
using JetBrains.Annotations;

namespace SharpNBT.ZLib
{
    /// <summary>
    /// Provides methods for the creation of a ZLib header as outlined by RFC-1950.
    /// </summary>
    /// <see href="https://datatracker.ietf.org/doc/html/rfc1950"/>
    public sealed class ZLibHeader
    {
        private byte compressionMethod; 
        private byte compressionInfo;   
        private byte fCheck;
        private byte fLevel;
        private byte fDict;

        /// <summary>
        /// Gets a flag indicating if this <see cref="ZLibHeader"/> represents a valid and supported ZLib format.
        /// </summary>
        public bool IsSupported { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ZLibHeader"/> class using the specified compression strategy.
        /// </summary>
        /// <param name="compressionLevel">The desired level of compression.</param>
        public ZLibHeader(CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            const byte FASTER = 0;
            const byte DEFAULT = 2;
            const byte OPTIMAL = 3;
            
            compressionMethod = 8; // Deflate algorithm
            compressionInfo = 7;   // Window size
            fDict = 0;             // false
            
            fLevel = compressionLevel switch
            {
                CompressionLevel.NoCompression => FASTER,
                CompressionLevel.Fastest => DEFAULT,
                CompressionLevel.Optimal => OPTIMAL,
                _ => throw new ArgumentOutOfRangeException(nameof(compressionLevel))
            };
        }
        
        private void RefreshFCheck()
        {
            var flg = (byte) (Convert.ToByte(fLevel) << 1);
			flg |= Convert.ToByte(fDict);

            fCheck = Convert.ToByte(31 - Convert.ToByte((CMF * 256 + flg) % 31));
            if (fCheck > 31)
                throw new ArgumentOutOfRangeException(nameof(fCheck), "Value cannot be greater than 31.");
        }

        /// <summary>
        /// Gets the computed "compression method and flags" (CMF) value of the header.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private byte CMF => (byte)((compressionInfo << 4) | compressionMethod);

        /// <summary>
        /// Gets the computed "flags" (FLG) value of the header.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private byte FLG => (byte)((fLevel << 6) | (fDict << 5) | fCheck);

        /// <summary>
        /// Computes and returns the CMF and FLG magic numbers associated with a ZLib header.
        /// </summary>
        /// <returns>A two element byte array containing the CMF and FLG values.</returns>
        [NotNull]
        public byte[] Encode() 
        {
            var result = new byte[2];
            RefreshFCheck();

            result[0] = CMF;
            result[1] = FLG;

            return result;
        }

        /// <summary>
        /// Calculates and returns a new <see cref="ZLibHeader"/> instance from the specified CMF and FLG magic bytes read from a ZLib header.
        /// </summary>
        /// <param name="cmf">The first byte of a ZLib header.</param>
        /// <param name="flg">The second byte of a ZLib header.</param>
        /// <returns>The decoded <see cref="ZLibHeader"/> instance.</returns>
        [NotNull]
        public static ZLibHeader Decode(int cmf, int flg)
        {
            var result = new ZLibHeader();
            cmf = cmf & 0x0FF;
			flg = flg & 0x0FF;
            
			result.compressionInfo = Convert.ToByte((cmf & 0xF0) >> 4);
            if (result.compressionInfo > 15)
                throw new ArgumentOutOfRangeException(nameof(result.compressionInfo), "Value cannot be greater than 15");
            
			result.compressionMethod = Convert.ToByte(cmf & 0x0F);
            if (result.compressionInfo > 15)
                throw new ArgumentOutOfRangeException(nameof(result.compressionMethod), "Value cannot be greater than 15");

			result.fCheck = Convert.ToByte(flg & 0x1F);
			result.fDict  = Convert.ToByte((flg & 0x20) >> 5);
			result.fLevel = Convert.ToByte((flg & 0xC0) >> 6);

			result.IsSupported = (result.compressionMethod == 8) && (result.compressionInfo == 7) && (((cmf * 256 + flg) % 31 == 0)) && (result.fDict == 0);

            return result;
        }
    }
}
