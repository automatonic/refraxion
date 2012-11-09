using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Refraxion.Model;

namespace Refraxion
{
    public partial class ModelBuilder
    {
        XmlDocument _TempDocument;
        ModelBuilder()
        {
            _TempDocument = new XmlDocument();
            _TempDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Assembly xmlns=\"http://devfuel.com/Refraxion\"/>");
        }

        protected void BuildFileComments(RxMemberInfo memberInfo)
        {
            string commentFileName = memberInfo.id + ".comments";
            foreach (string commentFile in this.InputDocumentationFolders.Select(folder => Path.Combine(folder, commentFileName)))
            {
                if (File.Exists(commentFile))
                {
                    BuildComments(memberInfo, File.ReadAllText(commentFile));
                    return;
                }
            }
            Log.LogWarning("Could not find comments file for \"{0}\"", memberInfo);
        }

        protected void BuildComments(RxMemberInfo memberInfo, XElement input)
        {
            if (input == null)
            {
                Log.LogWarning("\"{0}\" had no comments.", memberInfo.id);
                return;
            }
            BuildComments(memberInfo, input.Nodes().Aggregate("", (s, n) => n.ToString(SaveOptions.DisableFormatting)));
        }

        protected void BuildComments(RxMemberInfo memberInfo, string input)
        {
            memberInfo.comments = new RxMemberInfoComments();
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
            memberInfo.comments.Any = nodes.ToArray();
        }

        
    }
}
