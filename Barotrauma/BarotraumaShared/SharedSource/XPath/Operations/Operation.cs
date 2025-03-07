using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Barotrauma.XPath;

public enum Order
{
    Append,
    Prepend,
}

[XmlInclude(typeof(OperationPathed)),
 XmlInclude(typeof(OperationAttribute))]
public abstract class Operation
{
    private string _xpath;

    [XmlElement]
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

    [XmlAttribute("order")] public Order Order;

    [XmlArray("Predicates")] public List<Predicate> Predicates { get; set; }

    protected abstract bool ApplyOperation(XmlElement element, PatcherContext context);
    protected abstract string OperationType();

    protected Order GetOrder()
    {
        if (Order != null)
        {
            return Order;
        }

        return Order.Append;
    }

    protected virtual bool Validate(XmlElement element, PatcherContext context)
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
                                $"{context.ContentPackage.Name} Patcher {context.Name} XPath operation {OperationType()} query \"{XPath}\" in \"{context.ContentPath.RawValue}\" predicate \"{predicate}\" not satisfied.",
                                Color.Cyan);
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    DebugConsole.LogError(
                        $"Patcher {context.Name} XPath operation {OperationType()} predicate \"{predicate.XPath}\" is not valid: {ex.Message}",
                        Color.Yellow,
                        contentPackage: context.ContentPackage);
                }
            }
        }

        return true;
    }

    // protected XmlComment CreateComment(XmlElement element, PatcherContext context)
    // {
    //     return element.OwnerDocument.CreateComment(
    //         $"Operation: {this.OperationType()} XPath: {this.XPath}");
    // }

    public void Apply(XmlElement element, PatcherContext context)
    {
        try
        {
            if (XPath == null)
            {
                DebugConsole.LogError(
                    $"Patcher {context.Name} XPath operation {OperationType()} missing XPath tag in {context.ContentPath.RawValue}",
                    Color.Yellow,
                    contentPackage: context.ContentPackage);
                return;
            }

            if (!Validate(element, context))
            {
                return;
            }

            if (!Filter(element, context))
            {
                return;
            }

            if (!ApplyOperation(element, context))
            {
                if (context.IsDebug)
                {
                    DebugConsole.NewMessage(
                        $"{context.ContentPackage.Name} Patcher {context.Name} operation {OperationType()} query \"{XPath}\" did not match any elements",
                        Color.Cyan);
                }
            }
        }
        catch (Exception ex)
        {
            DebugConsole.LogError(
                $"Patcher {context.Name} XPath operation {OperationType()} query \"{XPath}\" in \"{context.ContentPath.RawValue}\" failed. Caused by {ex.Message}",
                Color.Red,
                contentPackage: context.ContentPackage);
        }
    }
}