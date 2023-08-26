using System;
using System.ComponentModel;
using System.IO;
using JetBrains.Annotations;

namespace SharpNBT;

/// <summary>
/// Arguments supplied when an event that can be handled by an event subscriber.
/// </summary>
[PublicAPI]
public class TagHandledEventArgs : HandledEventArgs
{
    /// <summary>
    /// Gets a constant describing the basic NBT type of the tag.
    /// </summary>
    public TagType Type { get; }
        
    /// <summary>
    /// Gets flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.
    /// </summary>
    public bool IsNamed { get; }
        
    /// <summary>
    /// Gets the stream being read from, positioned at the beginning of the tag payload.
    /// <para/>
    /// When handling this event, the stream position must be moved to the end of the payload, ready for the next tag to be parsed.
    /// </summary>
    public Stream Stream { get; }
        
    /// <summary>
    /// Gets or sets the resulting tag from this event being handled.
    /// </summary>
    /// <remarks>This property <b>must</b> set to a non-null value when <see cref="HandledEventArgs.Handled"/> is <see langword="true"/>.</remarks>
    public Tag? Result { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="TagHandledEventArgs"/> class.
    /// </summary>
    /// <param name="type">A constant describing the basic NBT type of the tag.</param>
    /// <param name="isNamed">Flag indicating if this tag is named, only <see langowrd="false"/> when a tag is a direct child of a <see cref="ListTag"/>.</param>
    /// <param name="stream">The stream being read from, positioned at the beginning of the tag payload.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is <see langword="null"/>.</exception>
    public TagHandledEventArgs(TagType type, bool isNamed, Stream stream)
    {
        Type = type;
        IsNamed = isNamed;
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }
}