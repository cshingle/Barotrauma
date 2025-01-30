using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("Affliction")]
public class AfflictionElement : Element
{
    [XmlAttribute("identifier")] public string Identifier { get; set; }

    protected override string GetIdentifier(XmlElement element)
    {
        return element.GetAttribute("identifier");
    }

    protected string GetContainer()
    {
        return "Afflictions";
    }

    protected override bool SingleElement()
    {
        return false;
    }

    public override string XPathSelect()
    {
        if (!string.IsNullOrEmpty(Identifier))
        {
            StringBuilder xpathBuilder = new StringBuilder();
            bool first = true;
            // Until I find a more performant way just chain together a bunch of or conditions with an absolute path
            foreach (string afflictionTag in Enum.GetNames<AfflictionType>())
            {
                if (!first)
                    xpathBuilder.Append("|");
                else
                    first = false;
                xpathBuilder.Append($"//Afflictions/{afflictionTag}[@identifier='{Identifier}']");
                xpathBuilder.Append($"|//Afflictions/Override/{afflictionTag}[@identifier='{Identifier}']");
            }
            return xpathBuilder.ToString();
        }

        if (!string.IsNullOrEmpty(XPath))
        {
            return XPath;
        }

        throw new PatcherException($"{GetContainer()} patching requires identifier attribute or XPath tag");
    }
}

// There is probably a way to generate this
public enum AfflictionType
{
    Affliction,
    AcidBurn,
    AfflictionHusk,
    AfflictionPsychosis,
    AfflictionSpaceHerpes,
    Bleeding,
    Bloodloss,
    BuffDurationIncrease,
    Burn,
    Infection,
    InternalDamage,
    OxygenLow,
    Pressure,
    Slow,
    Stun
}