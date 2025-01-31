using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Append to an attribute in the selected elements
/// </summary>
[XmlRoot("InsertAttribute")]
public class OperationInsertAttribute : OperationAttribute
{
    protected override bool ApplyOperation(XmlElement element, PatcherContext context)
    {
        bool flag = false;
        Order order = GetOrder();
        string attribute = Attribute;
        string comment = $"{context.ContentPackage.Name} - {OperationType()} - {attribute}";

        foreach (XmlNode xmlNode in GetOperationNodes(element))
        {
            flag = true;
            string attributeValue = GetValue();
            XmlAttribute xmlAttribute = xmlNode.Attributes[attribute];
            if (xmlAttribute != null)
            {
                if (xmlAttribute.Value.Length > 0)
                {
                    switch (order)
                    {
                        case Order.Append:
                            xmlAttribute.Value = $"{xmlAttribute.Value},{attributeValue}";
                            break;
                        case Order.Prepend:
                            xmlAttribute.Value = $"{attributeValue},{xmlAttribute.Value}";
                            break;
                    }
                }
                else
                {
                    xmlAttribute.Value = attributeValue;
                }
            }
            else
            {
                xmlAttribute = element.OwnerDocument.CreateAttribute(attribute);
                xmlAttribute.Value = attributeValue;
                xmlNode.Attributes.Append(xmlAttribute);
            }
            
            xmlNode.ParentNode.InsertBefore(
                xmlNode.OwnerDocument!.CreateComment(comment), xmlNode);
        }

        return flag;
    }

    protected override string OperationType()
    {
        return "InsertAttribute";
    }
}