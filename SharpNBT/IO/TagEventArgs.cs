using System;
using JetBrains.Annotations;

namespace SharpNBT.IO;

/// <summary>
/// Arguments for tag-related events.
/// </summary>
[PublicAPI]
public sealed class TagEventArgs : EventArgs
{
    /// <summary>
    /// Gets the NBT tag triggering the event.
    /// </summary>
    public Tag Tag { get; }
    
    /// <summary>
    /// Gets the type of the tag that is triggering the event.
    /// </summary>
    public TagType Type { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="TagEventArgs"/> class.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <param name="type">The type of the tag.</param>
    public TagEventArgs(Tag tag, TagType type)
    {
        Tag = tag;
        Type = type;
    }
}