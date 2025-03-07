using System.Collections.Generic;
using System.IO;

namespace Barotrauma.XPath;

/// <summary>
/// Stores the mod context for use thought the patching process
/// </summary>
public class PatcherContext
{
    public readonly ContentPackage ContentPackage;
    public readonly ContentPath ContentPath;
    public readonly ContentPath PatchedContentPath;
    public readonly string Name;
    public bool IsDebug = false;
    /// <summary>
    /// Track the identities of elements written to the IRenderedDocument to ensure we don't end up with duplicates
    /// </summary>
    public readonly HashSet<string> PatchedIdentities = new HashSet<string>();

    public PatcherContext(ContentPackage contentPackage, ContentPath contentPath, ContentPath patchedContentPath)
    {
        this.ContentPackage = contentPackage;
        this.ContentPath = contentPath;
        this.Name = Path.GetFileName(contentPath.RawValue);
        this.PatchedContentPath = patchedContentPath;
    }
}