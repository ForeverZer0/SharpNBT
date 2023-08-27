using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using SharpNBT.SNBT;
using Xunit;
using Xunit.Abstractions;

namespace SharpNBT.Tests
{
    public class StringifiedTest
    {
        private readonly ITestOutputHelper output;
        
        
        public StringifiedTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void BigOutput()
        {
            var tag = TestHelper.GetTag("bigtest.nbt", CompressionType.GZip);
            output.WriteLine(tag.Stringify(true));
        }

        [Fact]
        public void HelloWorldOutput()
        {
            var tag = TestHelper.GetTag("hello_world.nbt", CompressionType.None);
            output.WriteLine(tag.Stringify(true));
        }

        [Fact]
        public void ParseBugged()
        {
            var testString = "{Count:1b,id:\"minecraft:netherite_sword\",tag:{Damage:0,Enchantments:[{id:\"minecraft:looting\",lvl:3s},{id:\"minecraft:smite\",lvl:5s},{id:\"minecraft:sweeping\",lvl:3s}],RepairCost:7,display:{Name:'{\"extra\":[{\"text\":\"我是修改的名字\"}],\"text\":\"\"}'}}}";
            var tag = StringNbt.Parse(testString);
            output.WriteLine(tag.PrettyPrinted());
        }

        [Fact]
        public void ParseSmall()
        {
            const string testString = "{name1:123,name2:\"sometext1\",name3:{subname1:456,subname2:\"sometext2\"}}";
            var tag = StringNbt.Parse(testString);
            output.WriteLine(tag.PrettyPrinted());
        }

        [Fact]
        public void ParseBig()
        {
            using var stream = TestHelper.GetFile("bigtest.snbt", CompressionType.None);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var testString = reader.ReadToEnd();

            var tag = StringNbt.Parse(testString);
            output.WriteLine(tag.PrettyPrinted());
        }
    }
}