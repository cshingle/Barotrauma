using System.Collections.Generic;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("AfflictionPatcher")]
public class AfflictionPatcher : Patcher
{
    [XmlArray("Afflictions")]
    [XmlArrayItem("Affliction")]
    public List<AfflictionElement> Afflictions { get; set; }

    protected override IRenderedDocument CreateParent(IRenderedDocument renderedDoc)
    {
        return renderedDoc.CreateElement("Afflictions");
    }

    public override IEnumerable<Element> GetPatchedElements()
    {
        return Afflictions;
    }
}