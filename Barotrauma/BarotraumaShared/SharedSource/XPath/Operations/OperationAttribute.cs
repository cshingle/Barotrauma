using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Barotrauma.XPath;

[XmlInclude(typeof(OperationAddAttribute)),
 XmlInclude(typeof(OperationInsertAttribute)),
 XmlInclude(typeof(OperationReplaceAttribute)),
 XmlInclude(typeof(OperationRemoveAttribute))]
public abstract class OperationAttribute : Operation
{
    [XmlAttribute("attribute")] public string Attribute;
    [XmlElement(ElementName = "Value")]
    public string Value;

    public string GetValue()
    {
        if (Value == null)
        {
            return "";
        }
        return Value;
    }

    protected XmlNode[] GetOperationNodes(XmlElement element)
    {
        if (this.XPath == ".")
        {
            return new XmlNode[] { element };
        }

        return element.SelectNodes(this.XPath).Cast<XmlNode>().ToArray<XmlNode>();
    }
    
    protected override bool Validate(XmlElement element, PatcherContext context)
    {
        if (Attribute.IsNullOrEmpty())
        {
            DebugConsole.LogError(
                $"Attribute not set for operation {OperationType()} query \"{XPath}\" in \"{context.ContentPath.RawValue}\"",
                Color.Yellow,
                contentPackage: context.ContentPackage);
            return false;
        }

        return true;
    }
}
