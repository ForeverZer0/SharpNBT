# SharpNBT

[![.NET](https://github.com/ForeverZer0/SharpNBT/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ForeverZer0/SharpNBT/actions/workflows/dotnet.yml)

A CLS-compliant implementation of the Named Binary Tag (NBT) specifications (Java/Bedrock), written in pure C# with no external dependencies and targeting a wide variety of .NET implementations and languages on all platforms.

## Features

* **Java/Bedrock Support:** Supports all NBT protocols used by different versions of Minecraft, including: Java, Bedrock (file protocol), and Bedrock (network protocol), including full support for either GZip or ZLib compression.
* **Ease-of-use:** An intuitive API design, following the style and conventions of the .NET runtime, with full Intellisense for every public member: Spend more time being productive and less time digging through documentation.
* **Performance:**  Leverages the power of modern C# language features, including `Span` with `stackalloc`, `MemoryMarshal`, etc. This allows for a type-safe way to reinterpret raw buffers without pointers or making unnecessary copies of buffers, a common pitfall with serialization in type-safe languages.
* **Concurrency:** Includes standard async/await concurrency patterns for reading and writing.
* **Cross-platform and cross-language support:** Fully CLR compliant and build against .NET STandard 2.1, allowing supports for any CLR language (i.e. C#, Visual Basic, F#, etc.) for the following runtime versions or greater:
    * .NET Standard 2.1
    * .NET 5.0
    * .NET Core 3.0
    * Mono 6.4
    * Xamarin.iOS 12.16
    * Xamarin.Mac 5.16
    * Xamarin.Android 10.0
    * Unity 2021.2.0b6
* **Callbacks:** Can subscribe to events that get a callback as the parser steps through a stream to get immediate feedback without waiting for completion.

## Usage

### Reading

Reading an NBT document can be as simple as a one-liner:

```csharp
CompoundTag tag = NbtFile.Read("/path/to/file.nbt", FormatOptions.Java, CompressionType.AutoDetect);
```

## Contributing

Bug reports and pull requests are welcome on GitHub at https://github.com/ForeverZer0/SharpNBT. This project is intended to be a safe, welcoming space for collaboration, and contributors are expected to adhere to the [Contributor Covenant](http://contributor-covenant.org) code of conduct.

Pull requests are always welcome.

## License

The project is available as open source under the terms of the [MIT License](https://opensource.org/licenses/MIT).


## Code of Conduct

Everyone interacting in the SharpNBT project’s codebases, issue trackers, chat rooms and mailing lists is expected to follow the [code of conduct](https://github.com/ForeverZer0/SharpNBT/blob/master/CODE_OF_CONDUCT.md).