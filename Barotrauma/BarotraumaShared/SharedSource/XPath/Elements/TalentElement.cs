using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("Talent")]
public class TalentElement : IdentifierElement
{
    protected override string GetElement()
    {
        return "Talent";
    }

    protected override string GetContainer()
    {
        return "Talents";
    }
    
    protected override bool SingleElement()
    {
        return false;
    }
}