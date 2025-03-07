using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlInclude(typeof(TalentElement)),
 XmlInclude(typeof(ItemElement)),
 XmlInclude(typeof(CharacterElement))]
public abstract class IdentifierElement : Element
{
    [XmlAttribute("identifier")] public string Identifier { get; set; }

    [XmlAttribute("tags")]
    public string Tags { get; set; }

    protected abstract string GetElement();

    protected abstract string GetContainer();

    protected override string GetIdentifier(XmlElement element)
    {
        return element.GetAttribute("identifier");
    }
    
    public override string XPathSelect()
    {
        if (!string.IsNullOrEmpty(Identifier))
        {
            return $"/{GetContainer()}/{GetElement()}[@identifier='{Identifier}']|/{GetContainer()}/Override/{GetElement()}[@identifier='{Identifier}']";
        }

        if (!string.IsNullOrEmpty(Tags))
        {
            return $"/{GetContainer()}/{GetElement()}[contains(@tags,'{Tags}')]|/{GetContainer()}/Override/{GetElement()}[contains(@tags,'{Tags}')]";
        }

        if (!string.IsNullOrEmpty(XPath))
        {
            return XPath;
        }

        throw new PatcherException($"{GetElement()} patching requires identifier or tags attribute or XPath tag");
    }

    public override bool Validate(XmlElement element)
    {
        if (!string.IsNullOrEmpty(Identifier))
        {
            return Identifier.Equals(element.GetAttribute("identifier"));
        }

        if (!string.IsNullOrEmpty(Tags))
        {
            // Use a regex with optional spaces around the comma
            string[] tags = Regex.Split(element.GetAttribute("tags"), " *, *");
            return tags.Contains(Tags);
        }

        return true;
    }
}