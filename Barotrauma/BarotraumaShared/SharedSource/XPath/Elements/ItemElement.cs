using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("Item")]
public class ItemElement : IdentifierElement
{
    protected override string GetElement()
    {
        return "Item";
    }

    protected override string GetContainer()
    {
        return "Items";
    }
    
    protected override bool SingleElement()
    {
        return false;
    }
}