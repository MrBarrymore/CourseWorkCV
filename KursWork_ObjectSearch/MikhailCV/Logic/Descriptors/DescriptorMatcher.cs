using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Descriptors
{
    public static class DescriptorMatcher
    {
        private const double NextDistance = 0.8;

        public static List<ValueTuple<Descriptor, Descriptor>> Match(List<Descriptor> a, List<Descriptor> b)
        {
            var match = new List<(Descriptor, Descriptor)>();

            foreach (var d in a)
            {
                var best = Find(d, b);
                if (best != null)
                    match.Add(new ValueTuple<Descriptor, Descriptor>(d, best));
            }

            return match;
        }

        private static Descriptor Find(Descriptor descriptor, List<Descriptor> list)
        {
            var distances = list.Select(x => descriptor.Vector.DistanceTo(x.Vector)).ToList();

            var a = FindNearest(distances, -1);
            var b = FindNearest(distances, a);

            var r = distances[a] / distances[b];

            return r <= NextDistance ? list[a] : null;
        }

        private static int FindNearest(List<double> distances, int exclude)
        {
            var nearest = -1;
            for (var i = 0; i < distances.Count; i++)
                if (i != exclude && (nearest == -1 || distances[i] < distances[nearest]))
                    nearest = i;

            return nearest;
        }

        public static List<ValueTuple<Descriptor, Descriptor>> Nndr(List<Descriptor> a, List<Descriptor> b)
        {
            var match = new List<(Descriptor, Descriptor)>();

            foreach (var d in a)
            {
                var list = b.OrderBy(x => x.Vector.DistanceTo(d.Vector)).Take(2).ToList();

                if (list.Count >= 2)
                    if (list[0].Vector.DistanceTo(d.Vector) / list[1].Vector.DistanceTo(d.Vector) <= NextDistance)
                        match.Add(new ValueTuple<Descriptor, Descriptor>(d, list[0]));
            }

            return match;
        }
    }
}
