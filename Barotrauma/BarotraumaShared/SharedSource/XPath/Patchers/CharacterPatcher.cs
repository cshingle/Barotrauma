using System.Collections.Generic;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("CharacterPatcher")]
public class CharacterPatcher : Patcher
{
    [XmlArray("Characters")]
    [XmlArrayItem("Character")]
    public List<CharacterElement> Characters { get; set; }

    protected override IRenderedDocument CreateParent(IRenderedDocument renderedDoc)
    {
        return renderedDoc;
    }

    public override IEnumerable<Element> GetPatchedElements()
    {
        return Characters;
    }
}