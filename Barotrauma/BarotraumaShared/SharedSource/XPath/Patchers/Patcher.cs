using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Barotrauma.XPath;

[XmlInclude(typeof(TalentPatcher)),
 XmlInclude(typeof(ItemPatcher)),
 XmlInclude(typeof(AfflictionPatcher)),
 XmlInclude(typeof(CharacterPatcher))]
public abstract class Patcher
{
    [XmlAttribute("debug")] public string debug { get; set; }

    [XmlArray("Sources")]
    [XmlArrayItem("Source")]
    public List<Source> Sources { get; set; }

    public bool IsDebug()
    {
        return debug != null && debug.ToLower() == "true";
    }

    public abstract IEnumerable<Element> GetPatchedElements();

    private void Apply(List<Source> sources, IRenderedDocument renderedDoc, PatcherContext context)
    {
        foreach (Element element in GetPatchedElements())
        {
            element.Apply(sources, renderedDoc, context);
        }
    }

    protected abstract IRenderedDocument CreateParent(IRenderedDocument renderedDoc);

    public void ApplyPatch(PatcherContext context)
    {
        context.IsDebug = this.IsDebug();
        
        DebugConsole.NewMessage(
            $"{context.ContentPackage.Name} Applying {context.Name} patches to {context.PatchedContentPath.Value}",
            Color.Cyan);
        
        IRenderedDocument renderedDoc = new RenderedDocument(context);
        this.Apply(Sources, CreateParent(renderedDoc), context);
        renderedDoc.Save();
    }
}




