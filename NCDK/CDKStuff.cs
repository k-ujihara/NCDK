using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ["AtomicNumber"] = "AN",
            ["AtomTypeName"] = "N",
            ["BondOrderSum"] = "BOS",
            ["CovalentRadius"] = "CR",
            ["ElectronCount"] = "EC",
            ["FormalCharge"] = "FC",
            ["FormalNeighbourCount"] = "NC",
            ["Hybridization"] = "H",
            ["Id"] = "ID",
            ["ImplicitHydrogenCount"] = "H",
            ["MaxBondOrder"] = "MBO",
            ["Number"] = "N",
            ["Point2D"] = "2D",
            ["Point3D"] = "3D",
            ["StereoParity"] = "SP",
            ["Symbol"] = "S",
            ["Valency"] = "EV",
        };

        private static HashSet<string> SkipList { get; } = new HashSet<string>()
        {
            "Element",
            "IsPlaced",
            "IsVisited",
            "Name",
        };

        public static string ToString(object obj)
        {
            try
            {
                var type = obj.GetType();

                var sb = new StringBuilder();
                sb.Append(type.Name);
                sb.Append("(");
                sb.Append(obj.GetHashCode());

                foreach (var p in type.GetProperties())
                {
                    if (SkipList.Contains(p.Name))
                        continue;
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType))
                        continue;
                    if (!PropertyNameConvesionMap.TryGetValue(p.Name, out string name))
                        name = p.Name;
                    try
                    {
                        var v = p.GetValue(obj);
                        if (v != null)
                        {
                            var s = v.ToString();
                            sb.Append(", ");
                            sb.Append(name).Append(":");
                            sb.Append(s);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                sb.Append(")");

                return sb.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
