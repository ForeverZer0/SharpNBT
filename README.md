# SharpNBT

A CLS-compliant implementation of the Named Binary Tag (NBT) specification, written in pure C# with no external dependencies and targeting .NET Standard 2.1 to support a wide variety of .NET implementations on all platforms.

## Features

* **Ease-of-use:** The structure revolves around a class inherited from a standard `Stream` object, which works and behaves exactly as would be expected for reading/writing NBT tags, no need to waste time frequently consulting API documentation.
* **Performance:**  Leverages the power of C# 8.0 language features, including `Span`, `MemoryMarshal`, etc. This allows for a type-safe way to reinterpret raw buffers without pointers or making unnecessary copies of buffers, a common pitfall with serialization in a type-safe language.
* **Cross-platform and cross-language (CLR) support:** Supports any CLR language (i.e. C#, Visual Basic, F#, etc.) for the following runtime versions or greater:
    * .NET Standard 2.1
    * .NET 5.0
    * .NET Core 3.0
    * Mono 6.4
    * Xamarin.iOS 12.16
    * Xamarin.Mac 5.16
    * Xamarin.Android 10.0
    * Unity 2021.2.0b6
* No-dependencies. 
* Includes a `TagBuilder` class for an even more simple way of building a complete tag from scratch with POD types.