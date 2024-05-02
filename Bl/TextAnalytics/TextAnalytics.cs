using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using social_analytics.Bl.structures;
using Telegram.Td.Api;
using TelegramWrapper.Models;

namespace social_analytics.Bl.TextAnalytics
{
    public class TextAnalytics
    {
        static public IFrequencyDictionary<string> GetFrequenctGraph(string[] words)
        {
            IFrequencyDictionary<string> freq = new FrequencyDictionary<string>();
            foreach (var word in words)
            {
                freq.AddFrequency(word,1);
            }
            return freq;
        }
        static public IEnumerable<string> GetStringEntities(HashSet<string> ignoreWords = null, params string[] inputs)
        {
            List<string> output = new(inputs.Length);
            foreach (var input in inputs)
            {
                if (!string.IsNullOrEmpty(input))
                {
                    output.AddRange(Regex.Matches(input, @"[\w]+").Where(m => ignoreWords == null || !ignoreWords.Contains(m.Value)).Select(match => match.Value.ToLower()));
                }
            }
            return output;
        }
        static public void UpdateFrequencyDict<T>(IFrequencyDictionary<T> inputFrequency, params T[] values)
        {
            foreach (var input in values)
            {
                inputFrequency.AddFrequency(input, 1);
            }
        }
        static public Dictionary<T, FrequencyGraph<T>> GetGpaphs<T>(int neighborsFromSideCount, params T[] values)
        // if the values have distance <= neighborsFromSide, we consider them neighboring graphs
        {
            // 1 2 3 4 5 6 7 8
            Dictionary<T, FrequencyGraph<T>> graphDict = new();
            Queue<int> right = new(neighborsFromSideCount);
            Queue<int> left = new(neighborsFromSideCount);
            int currentIndex = 0;
            while (currentIndex + 1 < values.Length && right.Count() + 1 < neighborsFromSideCount)
            {
                right.Enqueue(++currentIndex);
            }
            for (currentIndex = 0; currentIndex < values.Length; currentIndex++)
            {
                if (currentIndex + neighborsFromSideCount < values.Length)
                {
                    right.Enqueue(currentIndex + neighborsFromSideCount);
                }

                FrequencyDictionary<T> neight = new FrequencyDictionary<T>();
                foreach (var index in left)
                {
                    neight.AddFrequency(values[index], 1);
                }
                foreach (var index in right)
                {
                    neight.AddFrequency(values[index], 1);
                }
                if (graphDict.ContainsKey(values[currentIndex]))
                {
                    graphDict[values[currentIndex]].Neightboors = neight.Plus(graphDict[values[currentIndex]].Neightboors as FrequencyDictionary<T>);
                }
                else
                {
                    graphDict.Add(values[currentIndex], new FrequencyGraph<T>(values[currentIndex], neight));
                }

                if (left.Count() >= neighborsFromSideCount)
                {
                    left.Dequeue();
                }
                left.Enqueue(currentIndex);

                if (right.Count() > 0)
                {
                    right.Dequeue();
                }

            }
            return graphDict;
        }

        static public double CalcAverage(params double[] values)
        {
            return values.Sum() / values.Length;
        }
        static public int CalcAverage(params int[] values)
        {
            return values.Sum() / values.Length;
        }
        static public (FrequencyGraph<T> graph, double rate)[] RateGpaphs<T>(params FrequencyGraph<T>[] graphs)
        {

            return graphs.Select(gr => (gr, (double)gr.Neightboors.Sum())).OrderBy(tp => -tp.Item2).ToArray();
        }
        static public (FrequencyGraph<T> graph, double rate)[] RateGpaphsByFrequency<T>(params FrequencyGraph<T>[] graphs)
        {

            return graphs.Select(gr => (gr, (double)gr.Neightboors.Sum() / gr.Neightboors.Count())).OrderBy(rate => -rate.Item2).ToArray();
        }
        static public Dictionary<T, (FrequencyGraph<T> graph, double probability)> RateGpaphsTextRank<T>(params FrequencyGraph<T>[] graphs)
        {

            Dictionary<T, (FrequencyGraph<T> graph, double probability)> graphsDict = new();
            foreach (var graph in graphs)
            {
                graphsDict[graph.Value] = (graph, (double)graph.Neightboors.Sum() / graph.Neightboors.Count());
            }
            for (int i = 0; i < 10; i++)
            {

                foreach (var rate in graphsDict.Values)
                {
                    double probabilty = 0;
                    var neights = rate.graph.Neightboors;
                    foreach (var neight in neights)
                    {
                        var neightGr = graphsDict[neight.Key];
                        probabilty += neightGr.probability / neightGr.graph.Neightboors.Sum() * (int)neightGr.graph.Neightboors.GetKeyCount(rate.graph.Value);
                    }
                    graphsDict[rate.graph.Value] = (rate.graph, probabilty);

                }
            }
            return graphsDict;
        }

    }

}
