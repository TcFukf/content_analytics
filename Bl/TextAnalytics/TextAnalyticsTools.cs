using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using social_analytics.Bl.structures;
using social_analytics.Bl.TextAnalytics.MathModel.Scales;
using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;
using Telegram.Td.Api;
using TelegramWrapper.Models;

namespace social_analytics.Bl.TextAnalytics
{
    public static class TextAnalyticsTools
    {
        public static HashSet<string> StopWords { get; set; } = null;
        public static int distanceAttenuationRate = 10;
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
        static public IEnumerable<List<string>> GetStringsFromFileByLines(string fullFilePath,string regexPatt = @"[а-яА-я]+[\-:]{0,1}[0-9]{0,3}")
        {
            List<string> buffer = new List<string>();
            int bfSize = 20_000;
            using (StreamReader sr = new(fullFilePath, TextFiles.EncodingDetector.DetectFileEncoding(fullFilePath)))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string filtredString = ( string.Join(" ", Regex.Matches(line,regexPatt).Where(match => match.Success).Select(match=>match.Value) ) );
                    if (!string.IsNullOrEmpty(filtredString))
                    {
                        buffer.Add(filtredString);
                    }
                    if (buffer.Count >= bfSize)
                    {
                        yield return buffer;
                        buffer.Clear();
                    }
                    line = sr.ReadLine();
                }
                yield return buffer;
            }
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

        static public double TagsSimilarity(double weightTagA1,double weightTagA2, double rarety = 0)
        {
            double distance = Math.Abs(weightTagA2 - weightTagA1);
            return Math.Min(weightTagA1,weightTagA2)/( Math.Max(weightTagA1, weightTagA2) - distance * RaretyBalancer(rarety)  );
        }
        static public double SimilarityWeightOfTheTagGroup(double tagsSimilarity, double weightTagsGroup)
        {
            return tagsSimilarity * weightTagsGroup;
        }
        static public double TagsVectorSimilarity(IEnumerable< (double tagAW1,double tagAW2, double tagsGroupWeight) > tags)
        {
            double totalSum = 0;
            double similaritySum = 0;
            foreach (var tag in tags)
            {
                totalSum += tag.tagsGroupWeight;
                similaritySum = SimilarityWeightOfTheTagGroup(TagsSimilarity(tag.tagAW1, tag.tagAW2,tag.tagsGroupWeight), tag.tagsGroupWeight);
            }
            return  similaritySum/totalSum;
        }
        /// <summary>
        /// its 1>= F(x) >=0 ,x->maxDist , F(x)->0
        /// monot slowly decreasing function
        /// </summary>
        /// <param name="basicPosition"></param>
        /// <param name="tagPosition"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        ///
        public static double SlowDecreasingRate(double distance,int maxDistance)
        {
            if (distance < 0)
            {
                throw new ArgumentException($"distance < 0 . genius??");
            }
            double rate =  1 - distance/maxDistance;
            return rate;

        }

        /// <summary>
        ///0<=F(x)<=1 . F(x) -> 1 if Wg->1 and versa
        /// </summary>
        /// <param name="WG"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        ///
        public static double RaretyBalancer(double raretyWeight,double dist = 2)
        {
            /// подумать все ли тут ок.....
            // в точке rarety/dist функция = 0.9 , менять увелич dist если нужно d0*F(rarety,d0) > d1*W(rarety,d1) if d0 > d1 
            double k = 1;
            if (raretyWeight > 0)
            {
                k = (10-1) * dist / raretyWeight;
            }
            return 1 - 1 / (k*raretyWeight+1);
        }

        private static IEnumerable<KeyValuePair<string,Tag>> GetSmallestVector(WordTagsVector left, WordTagsVector right)
        {
            if (right.Length > left.Length)
            {
                return left;
            }
            return right;
        }
        public static double CalculateTagsSimilarity(WordTagsVector mainVector, WordTagsVector tagVector)
        {
            double totalSimSum = 0;
            double simSum = 0;
            foreach (var tag in GetSmallestVector(mainVector,tagVector))
            {
                totalSimSum += tag.Value.GroupTagWeight;
                double weight = tagVector.GetByKey(tag.Key)?.TagWeight ?? 0;
                if (weight == 0)
                {
                    continue;
                }
                double similit = TagsSimilarity(tag.Value.TagWeight, weight);
                simSum += similit* tag.Value.GroupTagWeight;
            }
            return simSum / totalSimSum;
        }
    }

}
