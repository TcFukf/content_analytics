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
        public static HashSet<string> stopWords { get; set; } = null;
        static public IFrequencyDictionary<string> GetFrequenctGraph(string[] words)
        {
            IFrequencyDictionary<string> freq = new FrequencyDictionary<string>();
            foreach (var word in words)
            {
                freq.AddFrequency(word, 1);
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
        static public List<string> GetSortedTags(string text, WordSkye frequencyСomparator)
        {
            return TextAnalytics.GetStringEntities(stopWords, text).DistinctBy(tagText=>tagText).OrderBy(word => frequencyСomparator.GetKeyFrequency(word)).ToList();
        }
        static public Dictionary<string, (int position, double weight)> PublicGetTagsBasic(List<string> sortedTags, Func<string,double> weightCalc)
        {
            Dictionary<string, (int position, double weight)> tagsTable = new();
            for (int i = 0; i < sortedTags.Count; i++)
            {
                string word = sortedTags[i];
                if (!tagsTable.ContainsKey(word))
                {
                    tagsTable.Add(word, (i,weightCalc(word)) );
                }
            }
            return tagsTable;
        }
        static public double CalculateTagsSimilarity( Dictionary<string, (int position, double weight ) >  tagsTable, IEnumerable<(string tagKey,int tagPosition)> tags)
        {
            double similitary = 0;
            foreach (var tag in tags)
            {
                float distance = float.MaxValue;
                if (tagsTable.ContainsKey(tag.tagKey))
                {
                    distance = tagsTable[tag.tagKey].position - tag.tagPosition;
                    similitary += (1 / distance) * tagsTable[tag.tagKey].weight; ;
                }
            }
            return similitary;
        }
        static public double CalculateTagsSimilarity(Dictionary<string, (int position, double weight)> tagsTable, List<string> sortedTags)
        {
            int maxDistance = Math.Max(tagsTable.Count,sortedTags.Count);
            double similitary = 0;
            double totalSum = 0;
            foreach (var tag in tagsTable)
            {
                totalSum += tag.Value.weight;
            }
            for (int i = 0; i < sortedTags.Count; i++)
            {
                (string tagKey, int tagPosition) tag = (sortedTags[i], i);
                double distance = maxDistance;
                double weight = 0;
                if (tagsTable.ContainsKey(tag.tagKey))
                {
                    distance = Math.Abs(tagsTable[tag.tagKey].position - tag.tagPosition);
                    weight = tagsTable[tag.tagKey].weight;
                }
                double distanceRate = CalcDistanceСoefficient(distance, maxDistance);
                if (weight != 0)
                    similitary += (1 - distanceRate) * weight;
            }
            return similitary/totalSum;
        }
        /// <summary>
        /// при |basPos - tagPos| = 0 -> 0
        /// при |basPos - tagPos| >= maxDistance -> 1 и больше соотв
        /// </summary>
        /// <param name="basicPosition"></param>
        /// <param name="tagPosition"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        private static double CalcDistanceСoefficient(double distance,int maxDistance)
        {
            // чем БОЛЬШЕ attenuation  тем  ДОЛЬШЕ растет коэффицент.
            // когда distance = maxDistance коээфицент равен 1( вес * (1-1) = 0) , и вот чем : больше attenuation тем больше веса получаем  при тойже дистании  
            if (distance < 0)
            {
                throw new ArgumentException($"distance < 0 . eblan??");
            }
            int attenuation = 1000;
            double a = Math.Log(maxDistance,maxDistance+attenuation);
            double rate = distance / Math.Pow(distance+attenuation,a);
            return rate;

        }
    }

}
