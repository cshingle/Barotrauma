using System;
using Barotrauma.XPath;

namespace Barotrauma
{
    sealed class XPathTalentsFile : XPathPrefabFile<TalentsFile>
    {
        public XPathTalentsFile(ContentPackage contentPackage, ContentPath path) : base(contentPackage, path)
        {
        }

        protected override Type GetPatchedType()
        {
            return typeof(TalentPatcher);
        }

        protected override TalentsFile CreateContentFile(ContentPath patchedPath)
        {
            return new TalentsFile(this.ContentPackage, patchedPath);
        }
    }
}