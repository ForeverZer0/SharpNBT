using System.IO.Compression;
using SharpNBT.IO;

namespace SharpNBT.Test;

public class BigTest
{
    private Stream stream;
    private CompoundTag compound;
    private Dictionary<string, Type> keyTypes;
    
    [SetUp]
    public void Setup()
    {
        var file = File.OpenRead("Data/bigtest.nbt");
        stream = new GZipStream(file, CompressionMode.Decompress, false);
        var reader = new JavaTagReader(stream);
        compound = reader.Read<CompoundTag>();
    }

    [TearDown]
    public void TearDown()
    {
        stream?.Dispose();
    }
    
    [Test]
    public void TestTypes()
    {
        Assert.That(compound["nested compound test"], Is.TypeOf<CompoundTag>());
        Assert.That(compound["intTest"], Is.TypeOf<IntTag>());
        Assert.That(compound["byteTest"], Is.TypeOf<ByteTag>());
        Assert.That(compound["stringTest"], Is.TypeOf<StringTag>());
        Assert.That(compound["listTest (long)"], Is.TypeOf<ListTag>());
        Assert.That(compound["doubleTest"], Is.TypeOf<DoubleTag>());
        Assert.That(compound["floatTest"], Is.TypeOf<FloatTag>());
        Assert.That(compound["longTest"], Is.TypeOf<LongTag>());
        Assert.That(compound["listTest (compound)"], Is.TypeOf<ListTag>());
        Assert.That(compound["byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))"],Is.TypeOf<ByteArrayTag>());
        Assert.That(compound["shortTest"], Is.TypeOf<ShortTag>());
    }
    
    
    [TestCase("byteTest", (byte) 127)]
    [TestCase("shortTest", (short) 32767)]
    [TestCase( "intTest", 2147483647)]
    [TestCase("longTest", 9223372036854775807L)]
    [TestCase("floatTest", 0.49823147058486938F)]
    [TestCase("doubleTest", 0.49312871321823148)]
    [TestCase("stringTest", "HELLO WORLD THIS IS A TEST STRING \xc5\xc4\xd6!")]
    public void TestValueTags<T>(string keyName, T value)
    {
        if (!compound.TryGetValue(keyName, out var tag))
            Assert.Fail($"Could not find a child with key {keyName}.");

        var valueTag = tag as IValueTag<T>;
        if (valueTag is null)
            Assert.Fail("Tag is not of the specified value type.");
        
        Assert.That(valueTag!.Value, Is.EqualTo(value));
        
      
        
        

        
        /*
  TAG_Compound('Level'): 11 entries
  {
    TAG_List('listTest (compound)'): 2 entries
    {
      TAG_Compound(None): 2 entries
      {
        TAG_Long('created-on'): 1264099775885L
        TAG_String('name'): 'Compound tag #0'
      }
      TAG_Compound(None): 2 entries
      {
        TAG_Long('created-on'): 1264099775885L
        TAG_String('name'): 'Compound tag #1'
      }
    }
 
  }
         */
        // Assert.That(tag["nested compound test"], Is.TypeOf<CompoundTag>());
        // Assert.That(tag["listTest (long)"], Is.TypeOf<ListTag>());
        // Assert.That(tag["listTest (compound)"], Is.TypeOf<ListTag>());
        //
    }

    [Test]
    public void ListTestLong()
    {
        var tag = compound.Get<ListTag>("listTest (long)");
        Assert.That(tag.Count, Is.EqualTo(5));
        Assert.That(tag.ChildType, Is.EqualTo(TagType.Long));

        var expected = 11;
        for (var i = 0; i < tag.Count; i++, expected++)
        {
            var child = tag[i];
            Assert.That(child.Name, Is.Null);

            var longChild = child as LongTag;
            Assert.That(longChild, Is.Not.Null);
            Assert.That(longChild!.Value, Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void ListTestCompound()
    {
        var tag = compound.Get<ListTag>("listTest (compound)");
        Assert.That(tag.Count, Is.EqualTo(2));
        Assert.That(tag.ChildType, Is.EqualTo(TagType.Compound));

        void TestChild(CompoundTag c, long createdOn, string name)
        {
            Assert.That(c.Name, Is.Null);
            Assert.That(c.Get<LongTag>("created-on").Value, Is.EqualTo(createdOn));
            Assert.That(c.Get<StringTag>("name").Value, Is.EqualTo(name));
        }

        var entry = tag[0] as CompoundTag;
        Assert.That(entry, Is.Not.Null);
        TestChild(entry!, 1264099775885L, "Compound tag #0");
        
        entry = tag[1] as CompoundTag;
        Assert.That(entry, Is.Not.Null);
        TestChild(entry!, 1264099775885L, "Compound tag #1");
    }

    [Test]
    public void ByteArrayTest()
    {
        const string name =
            "byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))";
        var tag = compound.Get<ByteArrayTag>(name);
        
        for (var n = 0; n < 1000; n++)
        {
            var expected = (n * n * 255 + n * 7) % 100;
            Assert.That(tag[n], Is.EqualTo(expected));
        }
    }
}