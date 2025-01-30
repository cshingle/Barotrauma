using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Replace selected elements with value
/// </summary>
[XmlRoot("Replace")]
public class OperationReplace : OperationPathed
{
    protected override bool ApplyOperation(XmlElement element, PatcherContext context)
    {
        bool flag = false;
        string comment = $"{context.ContentPackage.Name} - {OperationType()} XPath: \"{this.XPath}\"";
        foreach (XmlNode xmlNode in GetOperationNodes(element))
        {
            XmlElement valueElement = Value.GetAsElement();
            flag = true;
            XmlNode parentNode = xmlNode.ParentNode;
            parentNode.InsertBefore(
                xmlNode.OwnerDocument!.CreateComment(comment), xmlNode);
            foreach (XmlNode childNode in valueElement.ChildNodes)
                parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(childNode, true), xmlNode);
            parentNode.RemoveChild(xmlNode);
        }

        return flag;
    }

    protected override string OperationType()
    {
        return "Replace";
    }
}