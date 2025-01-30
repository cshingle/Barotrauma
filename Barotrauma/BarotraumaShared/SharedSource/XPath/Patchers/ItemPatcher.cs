using System.Collections.Generic;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("ItemPatcher")]
public class ItemPatcher : Patcher
{
    [XmlArray("Items")]
    [XmlArrayItem("Item")]
    public List<ItemElement> Items { get; set; }

    protected override IRenderedDocument CreateParent(IRenderedDocument renderedDoc)
    {
        return renderedDoc.CreateElement("Items");
    }

    public override IEnumerable<Element> GetPatchedElements()
    {
        return Items;
    }
}