using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlInclude(typeof(OperationReplace)),
 XmlInclude(typeof(OperationAdd)),
 XmlInclude(typeof(OperationInsert)),
 XmlInclude(typeof(OperationRemove))]
public abstract class OperationPathed : Operation
{
    [XmlElement(typeof(XmlContainer), ElementName = "Value")]
    public XmlContainer Value;

    protected XmlNode[] GetOperationNodes(XmlElement element)
    {
        if (this.XPath == ".")
        {
            return new XmlNode[] { element };
        }

        return element.SelectNodes(this.XPath).Cast<XmlNode>().ToArray<XmlNode>();
    }
}