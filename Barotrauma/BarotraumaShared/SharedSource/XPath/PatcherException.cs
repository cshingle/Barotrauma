using System;

namespace Barotrauma.XPath;

public class PatcherException : Exception
{
    public PatcherException(string message)
        : base(message)
    {
    }

    public PatcherException(string message, Exception inner)
        : base(message, inner)
    {
    }
}