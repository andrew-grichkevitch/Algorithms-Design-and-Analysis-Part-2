using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MinimizingWeightedSum
{
    /// <summary>
    /// Code up the greedy algorithms from lecture for minimizing the weighted sum of completion times.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            var jobs = ReadData(@"..\..\jobs.txt");

            long sum = 0;
            long length = 0;
            foreach (var job in jobs.OrderByDescending(j => j.Item1 - j.Item2).ThenByDescending(j => j.Item1))
            {
                length += job.Item2;
                sum += length*job.Item1;
            }
            Console.WriteLine(sum);

            sum = 0;
            length = 0;
            foreach (var job in jobs.OrderByDescending(j => (double) j.Item1/j.Item2))
            {
                length += job.Item2;
                sum += length*job.Item1;
            }
            Console.WriteLine(sum);

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        private static List<Tuple<long, long>> ReadData(string path)
        {
            var data = new List<Tuple<long, long>>();

            foreach (var line in File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var numbers = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                if (numbers.Length <= 1) continue;
                data.Add(new Tuple<long, long>(numbers[0], numbers[1]));
            }

            return data;
        }
    }
}
