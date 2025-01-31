using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Add or replace an attribute from the selected elements
/// </summary>
[XmlRoot("ReplaceAttribute")]
public class OperationReplaceAttribute : OperationAddAttribute
{
    protected override string OperationType()
    {
        return "ReplaceAttribute";
    }
}
