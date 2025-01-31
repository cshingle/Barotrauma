using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Barotrauma.XPath;

[XmlInclude(typeof(IdentifierElement)),
 XmlInclude(typeof(AfflictionElement)),
 XmlInclude(typeof(CharacterElement))]
public abstract class Element
{
    [XmlAttribute("override")] public string Override;

    [XmlElement("XPath")] public string XPath;

    [XmlArray("Predicates")] public List<Predicate> Predicates { get; set; }

    [XmlElement(typeof(OperationAdd), ElementName = "Add"),
     XmlElement(typeof(OperationInsert), ElementName = "Insert"),
     XmlElement(typeof(OperationReplace), ElementName = "Replace"),
     XmlElement(typeof(OperationRemove), ElementName = "Remove"),
     XmlElement(typeof(OperationAddAttribute), ElementName = "AddAttribute"),
     XmlElement(typeof(OperationInsertAttribute), ElementName = "InsertAttribute"),
     XmlElement(typeof(OperationReplaceAttribute), ElementName = "ReplaceAttribute"),
     XmlElement(typeof(OperationRemoveAttribute), ElementName = "RemoveAttribute")]
    public List<Operation> Operations;

    public bool IsOverride()
    {
        return Override == null || Override.ToLower() == "true";
    }

    public abstract string XPathSelect();

    protected abstract string GetIdentifier(XmlElement element);

    /// <summary>
    /// Single element documents can return after a single element has been written to the RenderedDocument since no other elements are valid.
    /// </summary>
    /// <returns></returns>
    protected abstract bool SingleElement();

    /// <summary>
    /// Matching on stuff like tags can get messy. This allows us to double-check the matches
    /// </summary>
    /// <returns></returns>
    public virtual bool Validate(XmlElement element)
    {
        return true;
    }

    protected virtual bool Filter(XmlElement element, PatcherContext context)
    {
        if (this.Predicates != null)
        {
            foreach (Predicate predicate in this.Predicates)
            {
                try
                {
                    XmlNode predicateElement = element.SelectSingleNode(predicate.XPath);
                    if (predicateElement == null && !predicate.Invert)
                    {
                        if (context.IsDebug)
                        {
                            DebugConsole.NewMessage(
                                $"{context.ContentPackage.Name} Patcher {context.Name} predicate \"{predicate.XPath}\" not satisfied.",
                                Color.Cyan);
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    DebugConsole.LogError(
                        $"Patcher {context.Name} predicate \"{predicate.XPath}\" is not valid: {ex.Message}",
                        Color.Yellow,
                        contentPackage: context.ContentPackage);
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Render patched source XML to renderedDoc
    /// </summary>
    /// <param name="sources">List of Sources</param>
    /// <param name="renderedDoc">The document that the patched XML will be written to</param>
    /// <param name="context">Context is passed everywhere for logging and stuff</param>
    /// <exception cref="PatcherException"></exception>
    public void Apply(List<Source> sources, IRenderedDocument renderedDoc, PatcherContext context)
    {
        
        if (IsOverride())
        {
            renderedDoc = renderedDoc.CreateElement("Override");
        }
        
        // Render the XPathSelect once
        string xpath = XPathSelect();
        foreach (Source source in sources)
        {
            foreach (XmlDocument xmlDocument in source.GetXmlDocuments(context))
            {
                if (xmlDocument != null)
                {
                    XmlNodeList nodes = null;
                    try
                    {
                        nodes = xmlDocument.SelectNodes(xpath);
                    }
                    catch (Exception ex)
                    {
                        throw new PatcherException($"XPath query {xpath} Failed. Check your XPath query",
                            ex);
                    }

                    if (nodes != null)
                    {
                        foreach (XmlElement node in nodes)
                        {
                            if (this.Validate(node))
                            {
                                if (context.IsDebug)
                                {
                                    DebugConsole.NewMessage(
                                        $"{context.ContentPackage.Name} Patcher {context.Name} found element {GetIdentifier(node)}",
                                        Color.Cyan);
                                }

                                if (Filter(node, context))
                                {
                                    foreach (Operation operation in this.Operations)
                                    {
                                        operation.Apply(node, context);
                                    }

                                    // The Operations could have changed the identifier
                                    string identifier = GetIdentifier(node);
                                    if (context.PatchedIdentities.Add(identifier))
                                    {
                                        renderedDoc.Append($"Source: {source.GetDescription()} XPath: {xpath}",
                                            node);

                                        if (SingleElement())
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}