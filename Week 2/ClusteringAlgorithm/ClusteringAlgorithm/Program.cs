using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ClusteringAlgorithm
{
    internal class Program
    {
        private static void Main()
        {
            MaxSpacingSimple(@"..\..\clustering1.txt");
            MaxSpacingComplex(@"..\..\clustering_big.txt");

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        /// <summary>
        /// Clustering algorithm from lecture for computing a max-spacing k-clustering.
        /// 
        /// Task in this problem is to run the clustering algorithm from lecture on this data set, where the target number k of clusters is set to 4.
        /// What is the maximum spacing of a 4-clustering?
        /// </summary>
        /// <param name="path"></param>
        private static void MaxSpacingSimple(string path)
        {
            const int K = 4;

            long nodes;
            var distances = ReadData(path, out nodes);

            var clusters = new List<List<long>>();
            for (int i = 0; i < nodes; i++)
            {
                clusters.Add(new List<long> { i + 1 });
            }

            foreach (var source in distances.OrderBy(i => i.Item2))
            {
                var i = clusters.IndexOf(clusters.Single(c => c.Contains(source.Item1.Item1)));
                var j = clusters.IndexOf(clusters.Single(c => c.Contains(source.Item1.Item2)));

                if (i != j)
                {
                    clusters[i].AddRange(clusters[j]);
                    clusters.RemoveAt(j);

                    if (clusters.Count <= K) break;
                }
            }

            long minSpacing = int.MaxValue;
            for (var i = 0; i < clusters.Count - 1; i++)
            {
                for (var j = i + 1; j < clusters.Count; j++)
                {
                    foreach (var node1 in clusters[i])
                    {
                        foreach (var node2 in clusters[j])
                        {
                            var distance = distances.First(d =>
                                (d.Item1.Item1 == node1 && d.Item1.Item2 == node2) ||
                                (d.Item1.Item1 == node2 && d.Item1.Item2 == node1)).Item2;
                            if (distance < minSpacing) minSpacing = distance;
                        }
                    }
                }
            }
            Console.WriteLine(minSpacing);
        }

        /// <summary>
        /// This file describes a distance function (equivalently, a complete graph with edge costs). It has the following format:
        ///     [number_of_nodes]
        ///     [edge 1 node 1] [edge 1 node 2] [edge 1 cost]
        ///     [edge 2 node 1] [edge 2 node 2] [edge 2 cost]
        /// </summary>
        /// <example>
        /// For example, the third line of the file is "1 3 5250",
        /// indicating that the distance between nodes 1 and 3 (equivalently, the cost of the edge (1,3)) is 5250.
        /// </example>
        /// <param name="path"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static List<Tuple<Tuple<long, long>, long>> ReadData(string path, out long nodes)
        {
            nodes = 0;
            var data = new List<Tuple<Tuple<long, long>, long>>();

            foreach (var line in File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var n = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                if (n.Length < 3)
                {
                    nodes = n[0];
                }
                else
                {
                    data.Add(new Tuple<Tuple<long, long>, long>(new Tuple<long, long>(n[0], n[1]), n[2]));
                }
            }

            return data;
        }


        /// <summary>
        /// Clustering algorithm from lecture, but on a MUCH bigger graph.
        /// So big, in fact, that the distances (i.e., edge costs) are only defined implicitly, rather than being provided as an explicit list.
        /// 
        /// The question is: what is the largest value of k such that there is a k-clustering with spacing at least 3?
        /// That is, how many clusters are needed to ensure that no pair of nodes with all but 2 bits in common get split into different clusters?
        /// 
        /// The distance between two nodes u and v in this problem is defined as the Hamming distance - the number of differing bits between the two nodes' labels.
        /// </summary>
        /// <param name="path"></param>
        private static void MaxSpacingComplex(string path)
        {
            int[] masks;
            var nodes = ReadData(path, out masks);

            var hashtable = new Dictionary<BitVector32, BitVector32>();
            foreach (var vector in nodes)
            {
                hashtable[vector] = vector;
            }

            foreach (var vector in nodes)
            {
                var variants = GetVariants(vector, masks);

                int depth1;
                var leader1 = GetLeader(hashtable, vector, out depth1);

                foreach (var variant in variants)
                {
                    if (!hashtable.ContainsKey(variant)) continue;

                    int depth2;
                    var leader2 = GetLeader(hashtable, variant, out depth2);

                    if (!leader1.Equals(leader2))
                    {
                        if (depth1 < depth2)
                        {
                            hashtable[leader1] = hashtable[leader2];
                        }
                        else
                        {
                            hashtable[leader2] = hashtable[leader1];
                        }
                    }
                }
            }

            Console.WriteLine(hashtable.Keys.Count(key => hashtable[key].Equals(key)));
        }

        /// <summary>
        /// The format is:
        ///     [# of nodes] [# of bits for each node's label]
        ///     [first bit of node 1] ... [last bit of node 1]
        ///     [first bit of node 2] ... [last bit of node 2]
        /// </summary>
        /// <example>
        /// For example, the third line of the file "0 1 1 0 0 1 1 0 0 1 0 1 1 1 1 1 1 0 1 0 1 1 0 1" denotes the 24 bits associated with node #2.
        /// </example>
        /// <param name="path"></param>
        /// <param name="masks"></param>
        /// <returns></returns>
        private static HashSet<BitVector32> ReadData(string path, out int[] masks)
        {
            masks = null;
            var data = new HashSet<BitVector32>();

            foreach (var line in File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var numbers = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (numbers.Length < 3)
                {
                    masks = new int[int.Parse(numbers[1])];
                    for (var i = 0; i < masks.Length; i++)
                    {
                        if (i == 0) masks[i] = BitVector32.CreateMask();
                        else masks[i] = BitVector32.CreateMask(masks[i - 1]);
                    }
                }
                else if (masks != null)
                {
                    var vector = new BitVector32();
                    for (var i = 0; i < numbers.Length; i++)
                    {
                        vector[masks[i]] = numbers[i] == "1";
                    }
                    data.Add(vector);
                }
            }

            return data;
        }

        private static IEnumerable<BitVector32> GetVariants(BitVector32 vector, int[] masks)
        {
            for (var i = 0; i < masks.Length; i++)
            {
                var vector1 = new BitVector32(vector);
                vector1[masks[i]] = !vector1[masks[i]];
                yield return vector1;

                for (int j = i + 1; j < masks.Length; j++)
                {
                    var vector2 = new BitVector32(vector1);
                    vector2[masks[j]] = !vector2[masks[j]];
                    yield return vector2;
                }
            }
        }

        private static BitVector32 GetLeader(Dictionary<BitVector32, BitVector32> hashtable, BitVector32 vector, out int depth)
        {
            depth = 0;
            var leader = hashtable[vector];

            while (!leader.Equals(hashtable[leader]))
            {
                depth++;
                leader = hashtable[leader];
            }

            while (!vector.Equals(hashtable[vector]))
            {
                vector = hashtable[vector];
                hashtable[vector] = leader;
            }

            return leader;
        }
    }
}
