using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Replace selected elements
/// </summary>
[XmlRoot("Remove")]
public class OperationRemove : OperationPathed
{
    protected override bool ApplyOperation(XmlElement element, PatcherContext context)
    {
        bool flag = false;
        string comment = $"{context.ContentPackage.Name} - {OperationType()} XPath: \"{this.XPath}\"";

        foreach (XmlNode xmlNode in GetOperationNodes(element))
        {
            flag = true;
            xmlNode.ParentNode.InsertBefore(
                xmlNode.OwnerDocument!.CreateComment(comment), xmlNode);
            xmlNode.ParentNode!.RemoveChild(xmlNode);
        }

        return flag;
    }

    protected override string OperationType()
    {
        return "Remove";
    }
}
