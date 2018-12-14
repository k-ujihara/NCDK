using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NCDK
{
    internal static class CDKStuff
    {
        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(this IEnumerable<TSource> source)
        {
            switch (source)
            {
                case IReadOnlyList<TSource> list:
                    return list;
                case IList<TSource> list:
                    return new ReadOnlyCollection<TSource>(list);
                default:
                    return new List<TSource>(source);
            }
        }

        private static Dictionary<string, string> PropertyNameConvesionMap { get; } = new Dictionary<string, string>()
        {
            ["Atoms"] = "#A",
            ["Abundance"] = "AB",
            ["AtomicNumber"] = "AN",
            ["AtomTypeName"] = "N",
            ["Order"] = "#O",
            ["BondOrderSum"] = "BOS",
            ["Bonds"] = "#B",
            ["Charge"] = "C",
            ["CovalentRadius"] = "CR",
            ["CTerminus"] = "C",
            ["ElectronCount"] = "EC",
            ["ExactMass"] = "EM",
            ["FormalCharge"] = "FC",
            ["FractionalPoint3D"] = "F3D",
            ["FormalNeighbourCount"] = "NC",
            ["Hybridization"] = "H",
            ["Id"] = "ID",
            ["ImplicitHydrogenCount"] = "HC",
            ["LonePairs"] = "#LP",
            ["Mappings"] = "#M",
            ["MassNumber"] = "MN",
            ["MaxBondOrder"] = "MBO",
            ["MonomerName"] = "M",
            ["MonomerType"] = "T",
            ["NTerminus"] = "N",
            ["Point2D"] = "2D",
            ["Point3D"] = "3D",
            ["Reactions"] = "R",
            ["SingleElectrons"] = "#SE",
            ["SpaceGroup"] = "SG",
            ["Stereo"] = "#S",
            ["StereoElements"] = "#ST",
            ["StereoParity"] = "SP",
            ["StrandName"] = "N",
            ["StrandType"] = "T",
            ["Symbol"] = "S",
            ["Valency"] = "EV",
        };

        private static HashSet<string> SkipList { get; } = new HashSet<string>()
        {
            "Begin",
            "Builder",
            "Count",
            "ElectronContainer",
            "Element",
            "End",
            "Index",
            "IsPlaced",
            "IsVisited",
            "Name",
            "Notification",
            "SpaceGroup",
        };

        private static HashSet<string> ChemObjectsList { get; } = new HashSet<string>()
        {
            "Agents",
            "AssociatedAtoms",
            "Atoms",
            "Bonds",
            "LonePairs",
            "Mappings",
            "Products",
            "Reactants",
            "Reactions",
            "SingleElectrons",
            "StereoElements",
        };

        public static string ToString(object obj)
        {
            return new StringMaker(obj).ToString();
        }

        class StringMaker
        {
            private readonly List<int> outputtedList;
            private readonly object obj;

            public StringMaker(object obj, StringMaker parent = null)
            {
                this.outputtedList = parent == null ? new List<int>() : parent.outputtedList;
                switch (obj)
                {
                    case AtomRef o: obj = o.Deref(); break;
                    case BondRef o: obj = o.Deref(); break;
                }
                this.obj = obj;
            }

            public override string ToString()
            {
                var hashCode = obj.GetHashCode();
                var twice = outputtedList.Contains(hashCode);
                var sb = new StringBuilder();
                sb.Append(obj.GetType().Name);
                sb.Append("(");
                var list = new List<string> { hashCode.ToString() };
                if (obj is IEnumerable<IChemObject> v)
                {
                    var count = v.Count();
                    if (count > 0)
                    {
                        var s = string.Join(", ", v.Select(o => ToString(o)));
                        list.Add($"#:{count}[{s}]");
                    }
                    else
                    {
                        list.Add($"#:{count}");
                    }
                }
                if (!twice)
                {
                    outputtedList.Add(hashCode);
                    list.AddRange(PropertiesAsStrings(obj));
                }
                sb.Append(string.Join(", ", list));
                sb.Append(")");
                return sb.ToString();
            }

            private string ToString(object o)
            {
                if (o == null)
                    return "null";
                if (o is IChemObject)
                {
                    return new StringMaker((IChemObject)o, this).ToString();
                }
                else
                    return o.ToString();
            }

            public IEnumerable<string> PropertiesAsStrings(object obj)
            {
                var type = obj.GetType();
                foreach (var p in type.GetProperties())
                {
                    string str = null;
                    try
                    {
                        var ptype = p.PropertyType;
                        if (SkipList.Contains(p.Name))
                            continue;
                        if (!PropertyNameConvesionMap.TryGetValue(p.Name, out string name))
                            name = p.Name;
                        if (ChemObjectsList.Contains(p.Name))
                        {
                            if (!(p.GetValue(obj) is IEnumerable<IChemObject> v))
                                continue;
                            if (v.Count() == 0)
                                continue;
                            str = $"{name}:{v.Count()}[{string.Join(", ", v.Select(o => ToString(o)))}]";
                        }
                        else
                        {
                            if (ptype != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(ptype))
                                continue;
                            var v = p.GetValue(obj);
                            if (v == null)
                                continue;
                            if (ptype.IsEnum)
                            {
                                if ((int)v == 0)
                                    continue;
                            }
                            else if (ptype.IsValueType)
                            {
                                switch (v)
                                {
                                    case bool n: if (!n) continue; break;
                                    case char n: if (n == 0) continue; break;
                                    case short n: if (n == 0) continue; break;
                                    case int n: if (n == 0) continue; break;
                                    case long n: if (n == 0) continue; break;
                                    case float n: if (n == 0) continue; break;
                                    case double n: if (n == 0) continue; break;
                                    default: break;
                                }
                            }
                            var sb = new StringBuilder();
                            sb.Append($"{name}:");
                            if (ptype == typeof(string))
                                sb.Append("\"");
                            sb.Append(ToString(v));
                            if (ptype == typeof(string))
                                sb.Append("\"");
                            str = sb.ToString();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (str != null)
                        yield return str;
                }
                yield break;
            }
        }
    }
}
