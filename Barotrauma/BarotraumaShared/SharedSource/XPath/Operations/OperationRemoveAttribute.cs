using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Removes and attribute from the selected elements
/// </summary>
[XmlRoot("RemoveAttribute")]
public class OperationRemoveAttribute : OperationAttribute
{
    protected override bool ApplyOperation(XmlElement element, PatcherContext context)
    {
        bool flag = false;
        string attribute = Attribute;
        string comment = $"{context.ContentPackage.Name} - {OperationType()} - {attribute}";

        foreach (XmlNode xmlNode in GetOperationNodes(element))
        {
            flag = true;
            string attributeValue = GetValue();
            XmlAttribute xmlAttribute = xmlNode.Attributes[attribute];
            if (xmlAttribute != null)
            {
                xmlNode.Attributes.Remove(xmlAttribute);
                xmlNode.ParentNode.InsertBefore(
                    xmlNode.OwnerDocument!.CreateComment(comment), xmlNode);
            }
        }

        return flag;
    }

    protected override string OperationType()
    {
        return "RemoveAttribute";
    }
}