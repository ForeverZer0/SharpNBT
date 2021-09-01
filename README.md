# SharpNBT

[![.NET](https://github.com/ForeverZer0/SharpNBT/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ForeverZer0/SharpNBT/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/ForeverZer0/SharpNBT)](https://opensource.org/licenses/MIT)
[![Version](https://img.shields.io/nuget/v/SharpNBT.svg)](https://nuget.org/packages/SharpNBT)
[![Downloads](https://img.shields.io/nuget/dt/SharpNBT)](https://www.nuget.org/packages/SharpNBT)

![Java](https://img.shields.io/badge/Minecraft-Java-brightgreen)
![Bedrock](https://img.shields.io/badge/Minecraft-Bedrock-blue)

A CLS-compliant implementation of the Named Binary Tag (NBT) specifications (Java/Bedrock), written in pure C# with no external dependencies and targeting a wide variety of .NET implementations and languages on all platforms.

## Features

* **Java/Bedrock Support:** Supports all NBT protocols used by different versions of Minecraft, including: Java, Bedrock (file protocol), and Bedrock (network protocol), including full support for either GZip/ZLib compression, big/little endian, and variable length integers with optional ZigZag encoding.
* **Ease-of-use:** An intuitive API design, following the style and conventions of the .NET runtime, with full Intellisense for every member: Spend more time being productive and less time digging through documentation.
* **Performance:**  Leverages the power of modern C# language features, including `Span` with `stackalloc`, `MemoryMarshal`, etc. This allows for a type-safe way to reinterpret raw buffers without pointers or making unnecessary copies of buffers, a common pitfall with serialization in type-safe languages.
* **Concurrency:** Includes standard async/await concurrency patterns for reading and writing.
* **Cross-platform and cross-language support:** Fully CLR compliant and build against .NET Standard 2.1, allowing support for any CLR language (i.e. C#, Visual Basic, F#, etc.) for the following runtime versions or greater:
    * .NET Standard 2.1
    * .NET 5.0
    * .NET Core 3.0
    * Mono 6.4
    * Xamarin.iOS 12.16
    * Xamarin.Mac 5.16
    * Xamarin.Android 10.0
    * Unity 2021.2.0b6
* **Callbacks:** Can subscribe to events that get a callback as the parser steps through a stream to get immediate feedback without waiting for document completion. This allows subscribers to even parse the payload themselves and handle the event entirely.
* **String NBT**: Supports both generating and parsing arbitrary SNBT strings.

## Usage

Please see [the wiki](https://github.com/ForeverZer0/SharpNBT/wiki) for more detailed explanations. Feel free to improve it by contributing!

### Reading

At its simplest, reading an NBT document is one-liner:

```csharp
CompoundTag tag = NbtFile.Read("/path/to/file.nbt", FormatOptions.Java, CompressionType.AutoDetect);
```
### Writing

Likewise writing a completed NBT tag is a one-liner:
```csharp
// Assuming "tag" is a valid variable
NbtFile.Write("/path/to/file.nbt", tag, FormatOptions.BedrockFile, CompressionType.ZLib);
```

### Viewing

While there is functionality to output NBT to other human-readable formats like JSON, if you simply need to visualize a tag, there is a custom "pretty printed" output you can use:

[bigtest.nbt](https://raw.github.com/Dav1dde/nbd/master/test/bigtest.nbt) from https://wiki.vg/

```csharp

var tag = NbtFile.Read("bigtest.nbt", FormatOptions.Java, CompressionType.GZip);
Console.WriteLine(tag.PrettyPrinted())
```

#### Output

```
TAG_Compound("Level"): [11 entries]
{
    TAG_Long("longTest"): 9223372036854775807
    TAG_Short("shortTest"): 32767
    TAG_String("stringTest"): "HELLO WORLD THIS IS A TEST STRING ÅÄÖ!"
    TAG_Float("floatTest"): 0.49823147
    TAG_Int("intTest"): 2147483647
    TAG_Compound("nested compound test"): [2 entries]
    {
        TAG_Compound("ham"): [2 entries]
        {
            TAG_String("name"): "Hampus"
            TAG_Float("value"): 0.75
        }
        TAG_Compound("egg"): [2 entries]
        {
            TAG_String("name"): "Eggbert"
            TAG_Float("value"): 0.5
        }
    }
    TAG_List("listTest (long)"): [5 entries]
    {
        TAG_Long(None): 11
        TAG_Long(None): 12
        TAG_Long(None): 13
        TAG_Long(None): 14
        TAG_Long(None): 15
    }
    TAG_List("listTest (compound)"): [2 entries]
    {
        TAG_Compound(None): [2 entries]
        {
            TAG_String("name"): "Compound tag #0"
            TAG_Long("created-on"): 1264099775885
        }
        TAG_Compound(None): [2 entries]
        {
            TAG_String("name"): "Compound tag #1"
            TAG_Long("created-on"): 1264099775885
        }
    }
    TAG_Byte("byteTest"): 127
    TAG_Byte_Array("byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))"): [1000 elements]
    TAG_Double("doubleTest"): 0.4931287132182315
}
```

### Much More!

There is much more to SharpNBT than the examples above, please see [the wiki](https://github.com/ForeverZer0/SharpNBT/wiki) for more details, with real-world examples.

## Contributing

Bug reports and pull requests are welcome on GitHub at https://github.com/ForeverZer0/SharpNBT. This project is intended to be a safe, welcoming space for collaboration, and contributors are expected to adhere to the [Contributor Covenant](http://contributor-covenant.org) code of conduct.

Pull requests are always welcome.

## License

The project is available as open source under the terms of the [MIT License](https://opensource.org/licenses/MIT).

## Code of Conduct

Everyone interacting in the SharpNBT project’s codebases, issue trackers, chat rooms and mailing lists is expected to follow the [code of conduct](https://github.com/ForeverZer0/SharpNBT/blob/master/CODE_OF_CONDUCT.md).

## Special Thanks

This project would not be possible without all the contributors to the https://wiki.vg/ site and its maintainers, who have created an invaluable source of information for developers for everything related to the game of Minecraft.

---

If you benefit from this project, please consider supporting it by giving it a star on GitHub!