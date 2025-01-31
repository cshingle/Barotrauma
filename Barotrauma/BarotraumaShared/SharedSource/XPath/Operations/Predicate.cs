using System.Xml.Serialization;

namespace Barotrauma.XPath;

[XmlRoot("Predicate")]
public class Predicate
{
    private string _xpath;
    [XmlText]
    public string XPath
    {
        get => _xpath;
        set
        {
            if (value != null && !value.StartsWith('.'))
            {
                value = "." + value;
            }

            _xpath = value;
        }
    }

    [XmlAttribute("not")] public string Not;

    public bool Invert => !string.IsNullOrEmpty(Not) && Not.ToLower().Equals("true");
}