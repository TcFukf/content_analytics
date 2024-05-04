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
        public static HashSet<string> StopWords { get; set; } = null;
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
        static public IEnumerable<string> GetStringEntities(params string[] inputs)
        {
            return GetStringEntities(StopWords, inputs);
        }
        static public void UpdateFrequencyDict<T>(IFrequencyDictionary<T> inputFrequency, params T[] values)
        {
            foreach (var input in values)
            {
                inputFrequency.AddFrequency(input, 1);
            }}

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

        static public Dictionary<string, (int position, double weight)> GetBasicTags(List<string> sortedTags, Func<string,double> weightCalc)
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
        static public double CalculateTagsSimilarity(WordTags tagsTable, WordTags tags,ITagScales scales)
        {
            int maxDistance = Math.Max(tagsTable.Length, tags.Length);
            double similitary = 0;
            double totalSum = tagsTable.TotalSum(scales);
            
            for (int i = 0; i < tags.Length; i++)
            {
                (string tagKey, int tagPosition) tag = tags.GetByIndex(i);
                double distance = maxDistance;
                double weight = 0;

                (string tagKey, int tagPosition)? basicTag = tagsTable.GetByKey(tag.tagKey);
                if (basicTag.HasValue)
                {
                    distance = Math.Abs(basicTag.Value.tagPosition - tag.tagPosition);
                    weight = scales.CalcWeight(basicTag.Value.tagKey);
                }
                double distanceRate = CalcDistanceСoefficient(distance, maxDistance);
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
            // когда distance >= maxDistance коээфицент >= 1( вес * (1-1) = 0) , и вот чем : больше attenuation тем больше веса получаем  при тойже дистании  
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
