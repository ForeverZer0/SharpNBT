using System;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class TagBuilderTest
    {
        private const string FILE_PATH = "./Data/tagbuilder-test.nbt";
        
        private readonly ITestOutputHelper output;
        
        public TagBuilderTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Build()
        {
            var byteArray = new sbyte[] { sbyte.MinValue, -1, 0, 1, sbyte.MaxValue };
            var intArray = new int[] { int.MinValue, -1, 0, 1, int.MaxValue };
            var longArray = new long[] { long.MinValue, -1, 0, 1, long.MaxValue };

            var builder = new TagBuilder("Created with TagBuilder")
                .BeginCompound("nested compound test")
                    .BeginCompound("egg").AddString("name", "Eggbert").AddFloat("value", 0.5f).EndCompound()
                    .BeginCompound("ham").AddString("name", "Hampus").AddFloat("value", 0.75f).EndCompound()
                .EndCompound()
                .AddInt("integer test", 2147483647)
                .AddByte("byte test", 127)
                .AddString("string test", "HELLO WORLD THIS IS A TEST STRING \xc5\xc4\xd6!")
                .BeginList(TagType.Long, "List Test").AddLong(11).AddLong(12).AddLong(13).AddLong(14).AddLong(15).EndList()
                .AddDouble("Double test", 0.49312871321823148)
                .AddFloat("Float Test", 0.49823147058486938f)
                .AddLong("Long Test", 9223372036854775807L)
                .BeginList(TagType.Compound,"List Test (compound)")
                    .BeginCompound().AddLong("created-on", 1264099775885L).AddString("name", "tag1").EndCompound()
                    .BeginCompound().AddLong("created-on", 1264099775885L).AddString("name", "tag2").EndCompound()
                .EndList()
                .AddByteArray("byte array", new byte[] { 4, 127 })
                .AddShort("short test", 32767)
                .AddByteArray("Byte Array Test", byteArray)
                .AddIntArray("Int Array Test", intArray)
                .AddLongArray("Long Array Test", longArray);

            output.WriteLine("**** PRE-SERIALIZATION ****\n");
            var temp = builder.Create();
            output.WriteLine(temp.PrettyPrinted());

            using (var stream = NbtFile.OpenWrite(FILE_PATH))
                stream.WriteTag(temp);
            
            output.WriteLine("\n**** POST SERIALIZATION/DESERIALIZATION ****\n");
            using (var stream = NbtFile.OpenRead(FILE_PATH))
            {
                var compound = stream.ReadTag<CompoundTag>();
                output.WriteLine(compound.PrettyPrinted());
            }
        }
    }
}