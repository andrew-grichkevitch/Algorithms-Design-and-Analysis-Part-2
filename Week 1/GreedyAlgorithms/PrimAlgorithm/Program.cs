using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PrimAlgorithm
{
    /// <summary>
    /// Code up Prim's minimum spanning tree algorithm.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            long verticesCount;
            var edges = ReadData(@"..\..\edges.txt", out verticesCount);

            var tree = new List<Tuple<Tuple<long, long>, long>>();
            var vertices = new List<long> {1};

            while (vertices.Count < verticesCount)
            {
                Tuple<Tuple<long, long>, long> minEdge = null;

                foreach (var edge in edges.Where(e =>
                    (vertices.Contains(e.Item1.Item1) && !vertices.Contains(e.Item1.Item2)) ||
                    (vertices.Contains(e.Item1.Item2) && !vertices.Contains(e.Item1.Item1))))
                {
                    if (minEdge == null || edge.Item2 < minEdge.Item2) minEdge = edge;
                }

                if (!vertices.Contains(minEdge.Item1.Item1)) vertices.Add(minEdge.Item1.Item1);
                if (!vertices.Contains(minEdge.Item1.Item2)) vertices.Add(minEdge.Item1.Item2);

                edges.Remove(minEdge);
                tree.Add(minEdge);
            }

            Console.WriteLine(tree.Sum(e => e.Item2));

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        private static List<Tuple<Tuple<long, long>, long>> ReadData(string path, out long vertices)
        {
            vertices = 0;
            var data = new List<Tuple<Tuple<long, long>, long>>();

            foreach (var line in File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var n = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                if (n.Length < 3)
                {
                    vertices = n[0];
                }
                else
                {
                    data.Add(new Tuple<Tuple<long, long>, long>(new Tuple<long, long>(n[0], n[1]), n[2]));
                }
            }

            return data;
        }
    }
}
