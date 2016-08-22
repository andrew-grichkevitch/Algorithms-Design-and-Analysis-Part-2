using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace KnapsackProblem
{
    /// <summary>
    /// Code up the knapsack algorithm.
    /// You can assume that all numbers are positive. You should assume that item weights and the knapsack capacity are integers.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            int size;
            Tuple<int, int>[] items;

            items = ReadData(@"..\..\knapsack1.txt", out size); //2493893
            Console.WriteLine(DynamicProgrammingAlgorithm(items, size));

            items = ReadData(@"..\..\knapsack_big.txt", out size); //4243395
            Console.WriteLine(DynamicProgrammingAlgorithmOptimized(items, size));

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        private static int DynamicProgrammingAlgorithmOptimized(Tuple<int, int>[] items, int size)
        {
            int k;
            var a = new[] {new int[size + 1], new int[size + 1]};

            for (int i = 1; i <= items.Length; i++)
            {
                for (int j = 0; j <= size; j++)
                {
                    if (j == 0) continue;
                    if (items[i - 1].Item2 <= j)
                    {
                        k = j - items[i - 1].Item2;
                        a[1][j] = Math.Max(a[0][j], k >= 0 ? items[i - 1].Item1 + a[0][k] : items[i - 1].Item1);
                    }
                    else
                    {
                        a[1][j] = a[0][j];
                    }
                }

                Array.Copy(a[1], a[0], size + 1);
            }

            return a[a.Length - 1][a[0].Length - 1];
        }

        private static int DynamicProgrammingAlgorithm(Tuple<int, int>[] items, int size)
        {
            int k;
            var a = new int[items.Length + 1][];

            for (int i = 0; i <= items.Length; i++)
            {
                a[i] = new int[size + 1];
                if (i == 0) continue;

                for (int j = 0; j <= size; j++)
                {
                    if (j == 0) continue;
                    if (items[i - 1].Item2 <= j)
                    {
                        k = j - items[i - 1].Item2;
                        a[i][j] = Math.Max(a[i - 1][j], k >= 0 ? items[i - 1].Item1 + a[i - 1][k] : items[i - 1].Item1);
                    }
                    else
                    {
                        a[i][j] = a[i - 1][j];
                    }
                }
            }

            return a[a.Length - 1][a[0].Length - 1];
        }

        ///This file describes a knapsack instance, and it has the following format:
        ///[knapsack_size] [number_of_items]
        ///[value_1] [weight_1]
        ///[value_2] [weight_2]
        private static Tuple<int, int>[] ReadData(string path, out int size)
        {
            size = 0;
            var items = new List<Tuple<int, int>>();

            foreach (var line in File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var numbers = line.Split(' ').Select(int.Parse).ToArray();

                if (size == 0)
                {
                    size = numbers[0];
                }
                else
                {
                    items.Add(new Tuple<int, int>(numbers[0], numbers[1]));
                }
            }

            return items.ToArray();
        }
    }
}
