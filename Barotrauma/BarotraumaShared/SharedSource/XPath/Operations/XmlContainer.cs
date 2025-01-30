using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Barotrauma.XPath;

/// <summary>
/// Parse and store Value xml elements
/// </summary>
[XmlRoot("Value")]
public class XmlContainer : IXmlSerializable

{
    private string _outerXml;

    public XmlElement GetAsElement()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(_outerXml);
        return xmlDocument.DocumentElement;
    }

    public XmlSchema GetSchema()
    {
        return (null);
    }

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();
        _outerXml = reader.ReadOuterXml();
    }

    public void WriteXml(XmlWriter writer)
    {
        // We don't use serialization so no point in implementing this
        throw new NotImplementedException();
    }
}