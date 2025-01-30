using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("TalentPatcher")]
public class TalentPatcher : Patcher
{
    [XmlArray("Talents")]
    [XmlArrayItem("Talent")]
    public List<TalentElement> Talents { get; set; }

    protected override IRenderedDocument CreateParent(IRenderedDocument renderedDoc)
    {
        return renderedDoc.CreateElement("Talents");
    }

    public override IEnumerable<Element> GetPatchedElements()
    {
        return Talents;
    }
}