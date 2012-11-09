using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {

        protected void BuildFileComments(RxAssemblyInfo outputAssembly)
        {
            string inputDirectory = Path.GetDirectoryName(outputAssembly.InputAssembly.Location);
            string path = Path.Combine(inputDirectory, this.id) + ".comments";
            if (File.Exists(path))
            {
                BuildComments(File.ReadAllText(path));
            }
        }

        protected static void BuildComments(Compiler context, XElement input)
        {
            if (input == null)
            {
                context.LogWarning("\"{0}\" had no comments.", id);
                return;
            }
            BuildComments(input.Nodes().Aggregate("", (s, n) => n.ToString(SaveOptions.DisableFormatting)));
        }

        protected void BuildComments(string input)
        {
            comments = new RxMemberInfoComments();
            XmlDocumentFragment fragment = _TempDocument.CreateDocumentFragment();
            try
            {
                fragment.InnerXml = input;
            }
            catch (XmlException)
            {
                fragment.InnerXml = System.Security.SecurityElement.Escape(input);
            }

            List<XmlNode> nodes = fragment.ChildNodes.OfType<XmlNode>().Select(n => _TempDocument.ImportNode(n, true)).ToList();
            comments.Any = nodes.ToArray();
        }

        static XmlDocument _TempDocument;
        static RxMemberInfo()
        {
            _TempDocument = new XmlDocument();
            _TempDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Assembly xmlns=\"http://ghost-tasks.googlecode.com/Wiki/XmlDoc.xsd\"/>");
        }

        public class EqualityComparer : IEqualityComparer<RxMemberInfo>
        {
            public bool Equals(RxMemberInfo x, RxMemberInfo y)
            {
                return x.id.Equals(y.id);
            }

            public int GetHashCode(RxMemberInfo obj)
            {
                return obj.id.GetHashCode();
            }
        }

        public void SetUri(string uri)
        {
            AbsoluteUri = new Uri(uri);
            absoluteUri = AbsoluteUri.ToString();
        }

        protected void SetUri(Uri baseUri, string relativeUri)
        {
            AbsoluteUri = new Uri(baseUri, relativeUri);
            absoluteUri = AbsoluteUri.ToString();
        }

        protected void SetUri(RxMemberInfo parentMemberInfo, string relativeUri)
        {
            SetUri(parentMemberInfo.AbsoluteUri, relativeUri);
        }
    }

    public class RxMemberLiteral : RxMemberInfo
    {
    }

}
