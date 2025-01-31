using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Add or overwrite an attribute in the selected elements
/// </summary>
[XmlRoot("AddAttribute")]
public class OperationAddAttribute : OperationAttribute
{
    protected override bool ApplyOperation(XmlElement element, PatcherContext context)
    {
        bool flag = false;
        string attribute = Attribute;

        foreach (XmlNode xmlNode in GetOperationNodes(element))
        {
            flag = true;
            string attributeValue = GetValue();
            string comment = $"{context.ContentPackage.Name} - {OperationType()} - {attribute}";
            XmlAttribute xmlAttribute = xmlNode.Attributes[attribute];
            if (xmlAttribute != null)
            {
                xmlAttribute.Value = attributeValue;
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
        return "AddAttribute";
    }
}