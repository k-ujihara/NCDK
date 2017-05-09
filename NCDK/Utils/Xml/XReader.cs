//
// Copyright (C) 2016-2017  Kazuya Ujihara <chemformatter.com>
//

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NCDK.Util.Xml
{
    public abstract class XContentHandler
    {
        public virtual void DoctypeDecl(XDocumentType dtd) { }
        public virtual void StartDocument() { }
        public virtual void EndDocument() { }
        public virtual void CharacterData(XElement element) { }
        public virtual void StartElement(XElement element) { }
        public virtual void EndElement(XElement element) { }
    }

    public class XReader
    {
        public static string AttGetValue(IEnumerable<XAttribute> atts, string name)
        {
            XAttribute attribute = atts.Where(n => n.Name.LocalName == name).FirstOrDefault();
            return attribute == null ? null : attribute.Value;
        }

        public XContentHandler Handler { get; set; }

        public void Read(XDocument doc)
        {
            Handler.StartDocument();
            XElement e = doc.Root;
            Handler.StartElement(e);
            Handler.CharacterData(e);
            Read(e);
            Handler.EndElement(e);
            Handler.EndDocument();
        }

        private void Read(XElement element)
        {
            if (!element.HasElements)
                return;
            foreach (var e in element.Elements())
            {
                Handler.StartElement(e);
                Handler.CharacterData(e);
                Read(e);
                Handler.EndElement(e);
            }
        }
    }
}
