using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Barotrauma.IO;
using Microsoft.Xna.Framework;

namespace Barotrauma.XPath;

[XmlRoot("Source")]
public class Source
{
    // TODO extend this to support other other types in the future
    [XmlAttribute("required")] public string Required;
    [XmlAttribute("contentfile")] public string ContentFile { get; set; }

    private List<XmlDocument> _documents;

    private bool IsRequired => Required == null || Required.ToLower() != "false";


    public string GetDescription()
    {
        return ContentFile;
    }

    public List<XmlDocument> GetXmlDocuments(PatcherContext context)
    {
        if (_documents == null)
        {
            string path = null;
            try
            {
                if (ContentFile == null)
                {
                    throw new PatcherException($"{context.Name} source missing required attribute contentfile");
                }

                ContentPath contentPath = ContentPath.FromRaw(null, ContentFile);
                path = contentPath.FullPath;
                if (!File.Exists(path))
                {
                    if (IsRequired)
                    {
                        throw new PatcherException($"Source File {path} does not exist");
                    }
                    else
                    {
                        DebugConsole.NewMessage(
                            $"{context.ContentPackage.Name} Patcher {context.Name} source File {path} does not exist",
                            Color.Yellow);
                        return new List<XmlDocument>();
                    }
                }

                if (context.IsDebug)
                {
                    DebugConsole.NewMessage(
                        $"{context.ContentPackage.Name} loading source file {contentPath}",
                        Color.Cyan);
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                this._documents = new List<XmlDocument>() { doc };
            }
            catch (Exception e)
            {
                if (IsRequired)
                {
                    throw new PatcherException($"Source File {path} failed to load: {e.Message}");
                }
                else
                {
                    DebugConsole.NewMessage(
                        $"{context.ContentPackage.Name} Source File {path} failed to load: {e.Message}", Color.Cyan);
                    this._documents = new List<XmlDocument>();
                }
            }
        }

        return _documents;
    }
}