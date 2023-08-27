using System;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class ConversionTest
    {
        private readonly ITestOutputHelper output;
        private readonly CompoundTag tag;
        
        public ConversionTest(ITestOutputHelper output)
        {
            this.output = output;
            tag = TestHelper.GetTag("bigtest.nbt", CompressionType.GZip);
        }
        
        [Fact]
        public void JsonOutput1()
        {
            output.WriteLine(tag.ToJsonString(true));
        }

        [Fact]
        public unsafe void PinTest()
        {
            const int length = 69;
            var ary = Enumerable.Range(420, length);
            var intTag = new IntArrayTag("Foobar", ary);
            
            var sb = new StringBuilder();
            fixed (int* ptr = &intTag.GetPinnableReference())
            {
                for (var i = 0; i < length; i++)
                {
                    sb.Append(ptr[i]);
                    sb.Append(',');
                }
            }
            output.WriteLine(sb.ToString());
        }
    }
}