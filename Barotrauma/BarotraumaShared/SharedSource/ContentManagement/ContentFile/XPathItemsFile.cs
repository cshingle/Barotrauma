using System;
using Barotrauma.XPath;

namespace Barotrauma
{
    sealed class XPathItemsFile : XPathPrefabFile<ItemFile>
    {
        public XPathItemsFile(ContentPackage contentPackage, ContentPath path) : base(contentPackage, path)
        {
        }

        protected override Type GetPatchedType()
        {
            return typeof(ItemPatcher);
        }

        protected override ItemFile CreateContentFile(ContentPath patchedPath)
        {
            return new ItemFile(this.ContentPackage, patchedPath);
        }
    }
}