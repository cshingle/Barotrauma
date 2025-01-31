using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Insert value before or after selected node(s)
/// </summary>
[XmlRoot("Insert")]
public class OperationInsert : OperationPathed
{
    protected override bool ApplyOperation(XmlElement element, PatcherContext context)
    {
        bool flag = false;
        Order order = GetOrder();

        foreach (XmlNode xmlNode in GetOperationNodes(element))
        {
            XmlElement valueElement = Value.GetAsElement();
            flag = true;
            string comment = $"{context.ContentPackage.Name} - {OperationType()} XPath: \"{this.XPath}\"";
            XmlNode parentNode = xmlNode.ParentNode;
            // This really shouldn't be possible
            if (parentNode == null)
                throw new PatcherException($"Parent node is null: {xmlNode.OuterXml}");

            switch (order)
            {
                case Order.Append:
                    parentNode.InsertAfter(
                        parentNode.OwnerDocument!.CreateComment(comment + " - end"), xmlNode);
                    foreach (XmlNode childNode in valueElement.ChildNodes)
                        parentNode.InsertAfter(parentNode.OwnerDocument!.ImportNode(childNode, true), xmlNode);
                    parentNode.InsertAfter(
                        parentNode.OwnerDocument!.CreateComment(
                            comment), xmlNode);
                    break;
                case Order.Prepend:
                    parentNode.InsertBefore(
                        parentNode.OwnerDocument!.CreateComment(comment), xmlNode);
                    foreach (XmlNode childNode in valueElement.ChildNodes)
                        parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(childNode, true), xmlNode);
                    parentNode.InsertBefore(
                        parentNode.OwnerDocument!.CreateComment(comment + " - end"), xmlNode);
                    break;
            }
        }


        return flag;
    }

    protected override string OperationType()
    {
        return "Insert";
    }
}