using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using social_analytics.Bl.structures;
using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;

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
        static public IEnumerable<string> GetWordsFromStrings(HashSet<string> ignoreWords = null, params string[] inputs)
        {
            List<string> output = new(inputs.Length);
            foreach (var input in inputs)
            {
                if (!string.IsNullOrEmpty(input))
                {
                    output.AddRange(Regex.Matches(input, @"[а-яА-Я]+").Where(m => ignoreWords == null || !ignoreWords.Contains(m.Value)).Select(match => match.Value.ToLower()));
                }
            }
            return output;
        }
        static public IEnumerable<string> GetWordsFromStrings(params string[] inputs)
        {
            return GetWordsFromStrings(StopWords, inputs);
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
        static public void UpdateFrequencyDict<T>(IFrequencyDictionary<T> inputFrequency, params T[] dictItems)
        {
            foreach (var input in dictItems)
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
            return 1;
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
        public static double CalculateVectorsSimilarity(WordTagsVector mainVector, WordTagsVector tagVector)
        {
            //throw new Exception("думой как лучше сделать шобы весы считало и выполнялась пермутативность");
            // if V1[A1,B1,C1] , V2[A2,D1] - пройтись по обьединению множеств и тогда перестановки работают
            // или правильней посчитать сумму обьединений через  Vector1.TotalSum + Vector2.TotalSum  - intersectWeight(тут оно и считается вроде)

            if (mainVector.Length > tagVector.Length)
            {
                return CalculateVectorsSimilarity(tagVector,mainVector);
            }
            double intersectWeight = 0;
            foreach (var tag in mainVector)
            {
                Tag groupTag = tagVector.GetByKey(tag.Key);
                if (groupTag != null)
                {
                    double similarity = TagsSimilarity(tag.Value.TagWeight, groupTag.TagWeight, groupTag.GroupTagWeight);
                    intersectWeight += groupTag.GroupTagWeight*similarity;
                }
            }
            return intersectWeight / (mainVector.GroupSumTagsOfVector + tagVector.GroupSumTagsOfVector - intersectWeight);
            // sim = Sum( intersecr(WABS and WAD.. ) ) / (main.Vect + tag.Vect - intersect() )
        }
    }

}
