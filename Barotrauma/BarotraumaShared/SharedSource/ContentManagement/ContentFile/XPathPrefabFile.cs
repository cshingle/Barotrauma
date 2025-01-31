using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Barotrauma.XPath;
using Microsoft.Xna.Framework;

namespace Barotrauma
{
    internal abstract class XPathPrefabFile<T> : ContentFile where T : ContentFile
    {
        protected readonly ContentPath PatchedContentPath;
        private T _patchedContent;

        protected XPathPrefabFile(ContentPackage contentPackage, ContentPath path) : base(contentPackage, path)
        {
            this.PatchedContentPath = CreatePatchedContentPath();
        }

        protected abstract Type GetPatchedType();
        protected abstract T CreateContentFile(ContentPath patchedPath);

        /// <summary>
        /// Build rendered ContentPath
        /// </summary>
        /// <returns></returns>
        private ContentPath CreatePatchedContentPath()
        {
            string contentPath = Path.RawValue;
            string fileName = System.IO.Path.GetFileName(contentPath);
            string patchedFileName = Regex.Replace(fileName, @"[ _-]?xpath[ _-]?", "", RegexOptions.IgnoreCase);
            if (fileName.Equals(patchedFileName))
            {
                patchedFileName = Regex.Replace(fileName, @"[.]xml$", "_rendered.xml", RegexOptions.IgnoreCase);
            }
            return ContentPath.FromRaw(this.ContentPackage, contentPath.Replace(fileName, patchedFileName));
        }

        private Patcher LoadPatch()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(GetPatchedType());
                using StreamReader reader = new StreamReader(Path.Value);
                return (Patcher)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                // TODO this will be a common error. Make a better effort of improving the message.
                if (e.Message.Contains("There is an error in XML document (2, 2)"))
                {
                    DebugConsole.ThrowError(
                        $"XPathPrefabFile: Loading XPath {Path} from {ContentPackage} failed. Check that the Patcher file root XML tag matches the filelist.xml type.",
                        contentPackage: ContentPackage, appendStackTrace: false);
                }
                else
                {
                    DebugConsole.ThrowError(
                        $"XPathPrefabFile: Loading XPath {Path} from {ContentPackage} failed. Caused by {e.Message}",
                        e: e,
                        contentPackage: ContentPackage, appendStackTrace: true);
                }

                throw new PatcherException($"Patching {Path.FullPath} failed: {e.Message}", e);
            }
        }

        private void RenderPatchedContent()
        {
            Stopwatch watch = Stopwatch.StartNew();
            Patcher patcher = LoadPatch();

            try
            {
                patcher.ApplyPatch(new PatcherContext(ContentPackage, Path, PatchedContentPath));
                this._patchedContent = this.CreateContentFile(PatchedContentPath);
                DebugConsole.NewMessage(
                    $"{ContentPackage.Name} Rendered {PatchedContentPath.RawValue} in {watch.ElapsedMilliseconds} milliseconds", Color.White);
            }
            catch (Exception e)
            {
                DebugConsole.LogError($"Applying patch {Path.RawValue} failed: {e.Message}", Color.Red,
                    contentPackage: ContentPackage);
                throw new PatcherException($"Applying patch {Path.FullPath} failed: {e.Message}", e);
            }
            finally
            {
                watch.Stop();
            }
        }

        /// <summary>
        /// Called when this mod is enabled
        /// </summary>
        public override void LoadFile()
        {
            RenderPatchedContent();
            _patchedContent.LoadFile();
        }

        /// <summary>
        /// Called when this mod is disabled
        /// </summary>
        public override void UnloadFile()
        {
            _patchedContent?.UnloadFile();
        }

        /// <summary>
        /// Sort is called every time any mod is enabled/disabled
        /// </summary>
        public override void Sort()
        {
            _patchedContent?.Sort();
        }
        
        public override Md5Hash CalculateHash()
        {
            return Md5Hash.CalculateForFile(this.Path.Value, Md5Hash.StringHashOptions.IgnoreWhitespace);
        }
    }
}