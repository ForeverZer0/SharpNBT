using System;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class TagBuilderTest
    {
        private readonly ITestOutputHelper output;
        
        public TagBuilderTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static byte[] GetByteArray()
        {
            var bytes = new byte[1000];
            for (var i = 0; i < 1000; i++)
                bytes[i] = Convert.ToByte((i * i * 255 + i * 7) % 100);
            return bytes;
        }

        [Fact]
        public void CreateBigTest()
        {

            var builder = new TagBuilder("Level")
                .BeginCompound("nested compound test")
                    .BeginCompound("egg").AddString("name", "Eggbert").AddFloat("value", 0.5f).EndCompound()
                    .BeginCompound("ham").AddString("name", "Hampus").AddFloat("value", 0.75f).EndCompound()
                .EndCompound()
                .AddInt("iniTest", 2147483647)
                .AddByte("byteTest", 127)
                .AddString("stringTest", "HELLO WORLD THIS IS A TEST STRING \xc5\xc4\xd6!")
                .BeginList(TagType.Long, "listTest (long)")
                    .AddLong(11).AddLong(12).AddLong(13).AddLong(14).AddLong(15)
                .EndList()
                .AddDouble("doubleTest", 0.49312871321823148)
                .AddFloat("floatTest", 0.49823147058486938f)
                .AddLong("longTest", 9223372036854775807L)
                .BeginList(TagType.Compound, "listTest (compound)")
                    .BeginCompound().AddLong("created-on", 1264099775885L).AddString("name", "Compound tag #0").EndCompound()
                    .BeginCompound().AddLong("created-on", 1264099775885L).AddString("name", "Compound tag #1").EndCompound()
                .EndList()
                .AddByteArray("byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))", GetByteArray())
                .AddShort("shortTest", 32767);
            
            output.WriteLine(builder.Create().PrettyPrinted());
        }

        [Fact]
        public void CreateBigTest2()
        {
            var tb = new TagBuilder("Level");
            using (tb.NewCompound("nested compound test"))
            {
                using (tb.NewCompound("egg"))
                {
                    tb.AddString("name", "Eggbert");
                    tb.AddFloat("value", 0.5f);
                }

                using (tb.NewCompound("ham"))
                {
                    tb.AddString("name", "Hampus");
                    tb.AddFloat("value", 0.75f);
                }
            }
            
            tb.AddInt("iniTest", 2147483647);
            tb.AddByte("byteTest", 127);
            tb .AddString("stringTest", "HELLO WORLD THIS IS A TEST STRING \xc5\xc4\xd6!");

            using (tb.NewList(TagType.Long, "listTest (long"))
            {
                tb.AddLong(11);
                tb.AddLong(12);
                tb.AddLong(13);
                tb.AddLong(14);
                tb.AddLong(15);
            }
            
            tb.AddDouble("doubleTest", 0.49312871321823148);
            tb.AddFloat("floatTest", 0.49823147058486938f);
            tb.AddLong("longTest", 9223372036854775807L);

            using (tb.NewList(TagType.Compound, "listTest (compound)"))
            {
                using (tb.NewCompound(null))
                {
                    tb.AddLong("created-on", 1264099775885L);
                    tb.AddString("name", "Compound tag #0");
                }
                using (tb.NewCompound(null))
                {
                    tb.AddLong("created-on", 1264099775885L);
                    tb.AddString("name", "Compound tag #1");
                }
            }

            tb.AddByteArray("byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))", GetByteArray());
            tb.AddShort("shortTest", 32767);
                
            
            output.WriteLine(tb.Create().PrettyPrinted());
        }
    }
}