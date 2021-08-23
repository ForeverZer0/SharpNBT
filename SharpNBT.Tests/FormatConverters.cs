using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class FormatConverters
    {
        private readonly ITestOutputHelper output;
        private readonly CompoundTag compoundTag;
        
        public FormatConverters(ITestOutputHelper output)
        {
            this.output = output;
            using var stream = NbtStream.OpenRead("./Data/bigtest.nbt");
            compoundTag = stream.ReadTag<CompoundTag>();
        }

        [Fact]
        public void JsonTest()
        {
            
            using (var stream = File.OpenWrite("./Data/bigtest.json"))
            {
                var json = Encoding.UTF8.GetBytes(compoundTag.ToJsonString(true));
               stream.Write(json, 0, json.Length);
            }
        }

        [Fact]
        public void XmlTest()
        {
            using (var stream = File.OpenWrite("./Data/bigtest.xml"))
            {
                var xml = Encoding.UTF8.GetBytes(compoundTag.ToXmlString());
                stream.Write(xml, 0, xml.Length);
            }
        }
    }
}