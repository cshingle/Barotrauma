using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("Character")]
public class CharacterElement : Element
{
    [XmlAttribute("speciesname")] public string SpeciesName { get; set; }

    protected override string GetIdentifier(XmlElement element)
    {
        string speciesName = element.GetAttribute("speciesname");
        if (!string.IsNullOrEmpty(speciesName))
        {
            return speciesName;
        }
        return element.GetAttribute("SpeciesName");
    }

    public override string XPathSelect()
    {
        if (!string.IsNullOrEmpty(SpeciesName))
        {
            // This uses double slash so items in an Override tag will still work
            return
                $"/Character[@speciesname='{SpeciesName}']|/Character[@SpeciesName='{SpeciesName}']|/Charactervariant[@speciesname='{SpeciesName}']|/Charactervariant[@SpeciesName='{SpeciesName}']|/Override/Character[@speciesname='{SpeciesName}']|/Override/Character[@SpeciesName='{SpeciesName}']|/Override/Charactervariant[@speciesname='{SpeciesName}']|/Override/Charactervariant[@SpeciesName='{SpeciesName}']";
        }

        if (!string.IsNullOrEmpty(XPath))
        {
            return XPath;
        }

        throw new PatcherException("Character patching requires identifier attribute or XPath tag");
    }
    
    protected override bool SingleElement()
    {
        return true;
    }
}