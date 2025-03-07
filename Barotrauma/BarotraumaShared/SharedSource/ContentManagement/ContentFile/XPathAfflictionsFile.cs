using System;
using Barotrauma.XPath;

namespace Barotrauma
{
    sealed class XPathAfflictionsFile : XPathPrefabFile<AfflictionsFile>
    {
        public XPathAfflictionsFile(ContentPackage contentPackage, ContentPath path) : base(contentPackage, path)
        {
        }

        protected override Type GetPatchedType()
        {
            return typeof(AfflictionPatcher);
        }

        protected override AfflictionsFile CreateContentFile(ContentPath patchedPath)
        {
            return new AfflictionsFile(this.ContentPackage, patchedPath);
        }
    }
}