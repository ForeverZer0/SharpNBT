using System.ComponentModel;
using JetBrains.Annotations;

namespace SharpNBT.IO;

/// <summary>
/// Arguments for special handling of tag reading. Subscribers can choose to pass/handle the read completely.
/// </summary>
[PublicAPI]
public sealed class TagHandledEventArgs : HandledEventArgs
{
    /// <summary>
    /// Gets a value determining if the tag is named and if one should be read from the stream.
    /// </summary>
    public bool IsNamed { get; }
    
    /// <summary>
    /// Gets the type of the current tag in the stream.
    /// </summary>
    public TagType Type { get; }
    
    /// <summary>
    /// Gets or sets the result tag when handling the deserialization. The Handled property should be set to
    /// <see langword="true"/> when assigning a result so the reader knows not to process it.
    /// </summary>
    public Tag? Result { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="TagHandledEventArgs"/> class.
    /// </summary>
    /// <param name="named">Flag indicating if the tag has a name to read from the stream.</param>
    /// <param name="type">The type of the tag.</param>
    public TagHandledEventArgs(bool named, TagType type)
    {
        IsNamed = named;
        Type = type;
    }
}