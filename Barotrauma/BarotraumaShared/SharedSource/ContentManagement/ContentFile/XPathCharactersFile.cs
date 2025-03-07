using System;
using Barotrauma.XPath;

namespace Barotrauma
{
    sealed class XPathCharactersFile : XPathPrefabFile<CharacterFile>
    {
        public XPathCharactersFile(ContentPackage contentPackage, ContentPath path) : base(contentPackage, path)
        {
        }

        protected override Type GetPatchedType()
        {
            return typeof(CharacterPatcher);
        }

        protected override CharacterFile CreateContentFile(ContentPath patchedPath)
        {
            return new CharacterFile(this.ContentPackage, patchedPath);
        }
    }
}