using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NCDK
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().ToList())
            {
                if (type == typeof(Program))
                    continue;
                var main = type.GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(string[]) }, null);
                if (main != null)
                    main.Invoke(null, new object[] { Array.Empty<string>() });
            }
        }

        class MakeXml
        {
            static HashSet<string> excludeDirs = new HashSet<string>()
            {
                "bin", "obj",
            };

            static HashSet<string> excludeFileNames = new HashSet<string>()
            {
                "AssemblyInfo", "Program",
            };

            static void Main(string[] args)
            {
                //var source = new FileInfo(this.Host.TemplateFile).Directory;
                var source = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;

                var doc = new XDocument();
                var root = new XElement("Comments");
                doc.Add(root);
                AddElements(root, "NCDK", source);
                var str = doc.ToString();
                doc.Save("a.xml");
            }

            static Regex a = new Regex(@"^\s*#region([ ]+(?<name>[A-Za-z_\-]+))?\s*\n(?<content>.+?)^\s*#endregion\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.Multiline);

            static void AddElements(XElement parent, string parentId, DirectoryInfo dir)
            {
                foreach (var cd in dir.GetDirectories().Where(n => !excludeDirs.Contains(n.Name)))
                {
                    AddElements(parent, parentId + "." + cd.Name, cd);
                }
                foreach (var cs in dir.GetFiles()
                    .Where(fn => fn.Extension == ".cs")
                    .Where(fn => !excludeFileNames.Contains(Path.GetFileNameWithoutExtension(fn.Name))))
                {
                    string allContent;
                    using (var r = new StreamReader(cs.FullName))
                    {
                        allContent = r.ReadToEnd();
                    }
                    var ms = a.Matches(allContent);
                    if (ms.Count == 0)
                    {
                        var aCode = new XElement("Codes");
                        aCode.SetAttributeValue("id", parentId + "." + cs.Name);
                        var code = new XElement("code");
                        code.Add(new XText(allContent));
                        aCode.Add(code);
                        parent.Add(aCode);
                    }
                    else
                    {
                        foreach (var m in ms.Cast<Match>())
                        {
                            var name = m.Groups["name"].Value;
                            var content = m.Groups["content"].Value;
                            string id = parentId + "." + cs.Name;
                            if (!string.IsNullOrEmpty(name))
                                id = id + "+" + name;
                            var aCode = new XElement("Codes");
                            aCode.SetAttributeValue("id", id);
                            var code = new XElement("code");
                            code.Add(new XText(content));
                            aCode.Add(code);
                            parent.Add(aCode);
                        }
                    }
                }
            }
        }
    }
}
