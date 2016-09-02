using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AllPairsShortestPath
{
    internal class Program
    {
        /// <summary>
        /// Your task is to compute the "shortest shortest path".
        /// Precisely, you must first identify which, if any, of the three graphs have no negative cycles.
        /// For each such graph, you should compute all-pairs shortest paths and remember the smallest one (i.e., compute minu,v∈Vd(u,v),
        /// where d(u,v) denotes the shortest-path distance from u to v).
        ///  </summary>
        private static void Main()
        {
            for (int i = 1; i <= 3; i++)
            {
                var edges = ReadData($@"..\..\g{i}.txt");
                var pathes = FloydWarshallAlgorithm(edges);

                if (pathes == null)
                {
                    Console.WriteLine("has cycle!");
                }
                else
                {
                    Console.WriteLine(pathes.Min(p => p.Min()));
                    // Console.WriteLine(JohnsonsAlgorithm(edges).Min(p => p.Min()));
                }
            }

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        /// <summary>
        /// The first line indicates the number of vertices and edges, respectively.
        /// Each subsequent line describes an edge (the first two numbers are its tail and head, respectively) and its length (the third number).
        /// NOTE: some of the edge lengths are negative. NOTE: These graphs may or may not have negative-cost cycles.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static List<Tuple<int, int>>[] ReadData(string path)
        {
            List<Tuple<int, int>>[] edges = null;

            foreach (var line in File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var numbers = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                if (numbers.Length < 3)
                {
                    edges = new List<Tuple<int, int>>[numbers[0]];
                    for (int i = 0; i < edges.Length; i++)
                    {
                        if (edges[i] == null) edges[i] = new List<Tuple<int, int>>();
                    }
                }
                else
                {
                    // i - edge goes from, Tuple<int, int> - <edge goes to, weight>
                    edges?[numbers[0] - 1].Add(new Tuple<int, int>(numbers[1], numbers[2]));
                }
            }

            return edges;
        }


        private static long[] BellmanFordAlgorithm(IReadOnlyList<List<Tuple<int, int>>> edges, int s)
        {
            var d = new long[edges.Count];

            for (var i = 0; i < edges.Count; i++)
            {
                d[i] = i != s ? int.MaxValue : 0;
            }

            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < edges.Count; j++)
                {
                    foreach (var edge in edges[j])
                    {
                        var proposedLength = d[j] + edge.Item2;
                        if (d[edge.Item1 - 1] > proposedLength)
                        {
                            if (i + 1 == edges.Count) return null;
                            d[edge.Item1 - 1] = proposedLength;
                        }
                    }
                }
            }

            return d;
        }

        private static long[][] FloydWarshallAlgorithm(IReadOnlyList<List<Tuple<int, int>>> edges)
        {
            var w = new long[edges.Count][];

            // initialization
            for (int i = 0; i < edges.Count; i++)
            {
                var vertex = edges[i];
                w[i] = new long[edges.Count];

                for (int j = 0; j < edges.Count; j++)
                {
                    var edge = vertex.SingleOrDefault(e => e.Item1 == j + 1);
                    w[i][j] = i == j ? 0 : (edge?.Item2 ?? int.MaxValue);
                }
            }

            // algorithm main loop
            for (int k = 0; k < w.Length; k++)
            {
                for (int i = 0; i < w.Length; i++)
                {
                    for (int j = 0; j < w.Length; j++)
                    {
                        if (w[i][j] > w[i][k] + w[k][j])
                            w[i][j] = w[i][k] + w[k][j];
                    }
                }
            }

            // check for negative cycles
            return w.Where((a, i) => a[i] != 0).Any() ? null : w;
        }

        private static long[] DijkstraAlgorithm(IReadOnlyList<List<Tuple<int, int>>> edges, int v)
        {
            throw new NotImplementedException("Dijkstra's algorithm");
        }

        private static long[][] JohnsonsAlgorithm(List<Tuple<int, int>>[] edges)
        {
            // form new graph by adding new vertex s
            var extEdges = new List<Tuple<int, int>>[edges.Length + 1];
            Array.Copy(edges, extEdges, edges.Length);

            // adding new edges from s to all vertices with length 0
            extEdges[edges.Length] = new List<Tuple<int, int>>();
            for (int i = 0; i < edges.Length; i++)
            {
                extEdges[edges.Length].Add(new Tuple<int, int>(i + 1, 0));
            }

            // run Bellman–Ford algorithm on new graph
            var p = BellmanFordAlgorithm(extEdges, edges.Length);
            if (p == null) return null;

            // reweighting all edges in original graph
            var rwEdges = new List<Tuple<int, int>>[edges.Length];
            Array.Copy(edges, rwEdges, edges.Length);

            for (int i = 0; i < edges.Length; i++)
            {
                for (int j = 0; j < edges[i].Count; j++)
                {
                    var w = edges[i][j].Item2 + (int) p[i] - (int) p[edges[i][j].Item1 - 1];
                    edges[i][j] = new Tuple<int, int>(edges[i][j].Item1, w);
                }
            }

            // run Dijkstra's algorithm for each vertex
            var d = new long[edges.Length][];
            for (int i = 0; i < edges.Length; i++)
            {
                d[i] = DijkstraAlgorithm(new ArraySegment<List<Tuple<int, int>>>(rwEdges), i + 1);

                // reweighting all paths back
                for (int j = 0; j < edges.Length; j++)
                {
                    d[i][j] = d[i][j] - (int) p[i] + (int) p[j];
                }
            }

            return d;
        }
    }
}
