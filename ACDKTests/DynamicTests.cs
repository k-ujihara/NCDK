using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace ACDK
{
    [TestClass()]
    public class DynamicTests
    {
        class TypeAndParamerterName
        {
            public string TypeName { get; set; }
            public string ParameterName { get; set; }

            public TypeAndParamerterName(string typeName, string parameterName)
            {
                this.TypeName = typeName;
                this.ParameterName = parameterName;
            }

            public override string ToString()
            {
                return TypeName + " " + ParameterName;
            }
        }

        [TestMethod()]
        public void MakeWrapperMethodTest()
        {
            string a;
            Assert.AreNotEqual(null, a = MakeWrapperMethod("int A()"));
            Assert.AreNotEqual(null, a = MakeWrapperMethod("IAtom A()"));
            Assert.AreNotEqual(null, a = MakeWrapperMethod("IAtom A(string b)"));
            Assert.AreNotEqual(null, a = MakeWrapperMethod("IAtom A(IAtom b)"));
        }

        const string RN_Return = "return";
        static readonly Regex re = new Regex(@"^\b(?<return>\w+)\s+(?<name>\w+)\(\)$", RegexOptions.Compiled);
        static readonly Regex re2 = new Regex(@"^\b(?<return>\w+)\s+(?<name>\w+)\((?<typeandparam>\w+ \w+)(\, (?<typeandparam>\w+ \w+))*\)$", RegexOptions.Compiled);
        private static readonly HashSet<string> primitiveTypeNames = new HashSet<string>()
        { "void", "bool", "short", "int", "long", "float", "double", "string", };
        bool IsPrimitive(string typeName) => primitiveTypeNames.Contains(typeName);

        private string MakeWrapperMethod(string methodDefine)
        {
            string a;
            string methodName = null;
            string returnTypeName = null;
            List<TypeAndParamerterName> typeAndParameters = new List<TypeAndParamerterName>();
            string str = methodDefine;
            Match match;
            if ((match = re.Match(str)).Success)
            {
                methodName = match.Groups["name"].Value;
                returnTypeName = match.Groups["return"].Value;
            }
            if ((match = re2.Match(str)).Success)
            {
                methodName = match.Groups["name"].Value;
                returnTypeName = match.Groups["return"].Value;
                foreach (Capture tp in match.Groups["typeandparam"].Captures)
                {
                    var tpa = tp.Value.Split(' ');
                    typeAndParameters.Add(new TypeAndParamerterName(tpa[0], tpa[1]));
                }
            }
            a = MakeMethodWrapper(methodName, returnTypeName, typeAndParameters);
            return a;
        }

        private string MakeMethodWrapper(string methodName, string returnTypeName, IEnumerable<TypeAndParamerterName> parameters)
        {
            var ss = new List<string>();
            foreach (var tp in parameters)
            {
                string t = tp.TypeName;
                string p = tp.ParameterName;
                string s = null;
                s = !IsPrimitive(t) ? "(W_" + t + ")" + p + ").Object" : p;
                ss.Add(s);
            }

            var sb = new StringBuilder();
            sb.
                Append("public ").
                Append(returnTypeName).
                Append(' ').
                Append(methodName).
                Append("(");
            sb.Append(string.Join(", ", parameters));
            sb.Append(") => ");
            if (!IsPrimitive(returnTypeName))
            {
                sb.Append("new ");
                sb.Append("W_" + returnTypeName).Append("(");
            }
            sb.Append("Object.").Append(methodName).Append("(");
            sb.Append(string.Join(", ", ss));
            sb.Append(")");
            if (!IsPrimitive(returnTypeName))
            {
                sb.Append(")");
            }
            sb.Append(";");

            return sb.ToString();
        }
    }
}
