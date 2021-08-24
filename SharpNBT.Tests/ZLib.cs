using System.IO;
using System.IO.Compression;
using System.Text;
using SharpNBT.ZLib;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class ZLib
    {
        private readonly ITestOutputHelper console;
        
        public ZLib(ITestOutputHelper output)
        {
            console = output;
        }
        
        
        [Fact]
        public void Compress()
        {
 
            var text = File.ReadAllText("./Data/bigtest.json");
            
            using (var output = File.OpenWrite("./Data/bigtest.zlib"))
            {
                using (var zlib = new ZLibStream(output, CompressionLevel.Fastest, true))
                {
                    var bytes = Encoding.UTF8.GetBytes(text);
                    zlib.Write(bytes, 0, bytes.Length);
                }
            }


            using (var input = File.OpenRead("./Data/bigtest.zlib"))
            {
                using (var zlib = new ZLibStream(input, CompressionMode.Decompress))
                {
                    var sb = new StringBuilder();
                    var buffer = new byte[1024];
                    
                    while (true)
                    {
                        var read = zlib.Read(buffer, 0, 1024);
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, read));

                        if (read < 1024)
                            break;
                    }
                    
                    console.WriteLine(sb.ToString());
                    
                }
            }
        }
    }
}