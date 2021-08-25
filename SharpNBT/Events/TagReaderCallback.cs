using System;

namespace SharpNBT
{
    /// <summary>
    /// Handler for events used with the <see cref="TagReader"/> class.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="EventArgs"/>.</typeparam>
    /// <param name="reader">The <see cref="TagReader"/> instance invoking the event.</param>
    /// <param name="args">Any relevant args to be supplied with the callback,</param>
    public delegate void TagReaderCallback<in T>(TagReader reader, T args) where T : EventArgs;
}