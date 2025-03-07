using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Adds Value as new child node(s)
/// </summary>
[XmlRoot("Add")]
public class OperationAdd : OperationPathed
{
    protected override bool ApplyOperation(XmlElement element, PatcherContext context)
    {
        bool flag = false;
        Order order = GetOrder();
        {
            foreach (XmlNode xmlNode in GetOperationNodes(element))
            {
                XmlElement valueElement = Value.GetAsElement();
                flag = true;
                string comment = $"{context.ContentPackage.Name} - {OperationType()} XPath: \"{this.XPath}\"";
                
                switch (order)
                {
                    case Order.Append:
                        xmlNode.AppendChild(xmlNode.OwnerDocument!.CreateComment(comment));
                        foreach (XmlNode childNode in valueElement.ChildNodes)
                        {
                            xmlNode.AppendChild(xmlNode.OwnerDocument!.ImportNode(childNode, true));
                        }
                        xmlNode.AppendChild(xmlNode.OwnerDocument!.CreateComment(comment + " - end"));
                        break;
                    case Order.Prepend:
                        xmlNode.AppendChild(xmlNode.OwnerDocument!.CreateComment(comment));
                        foreach (XmlNode childNode in valueElement.ChildNodes)
                            xmlNode.PrependChild(xmlNode.OwnerDocument!.ImportNode(childNode, true));
                        xmlNode.AppendChild(xmlNode.OwnerDocument!.CreateComment(comment + " - end"));
                        break;
                }
            }
        }

        return flag;
    }

    protected override string OperationType()
    {
        return "Add";
    }
}