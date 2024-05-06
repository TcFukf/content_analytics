using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using social_analytics.Bl.structures;
using social_analytics.Bl.TextAnalytics.MathModel;
using social_analytics.Bl.TextAnalytics.MathModel.Scales;
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
        // чтобы алгоритм работал правильно нужно чтобы WordTags(вектор получается) был унифицирован -
        // все елементы отображены на множ-во как у scales ведь они и так от него зависимы(слова сортируются им)
        // А СЕЙЧАС ПОДРАЗУМЕВАЮТСЯ ТО ОТОБРАЖЕНИЯ А ИСПОЛЬЗУЮТСЯ ИСХОДНЫЕ СЛОВА.
        // вся я хаватььбюв
        // МБ УБРАТЬ DI ОТ SCALES И ОСТАВИТЬ ТОКА ВЕКТОРЫ И ЛОГИКУ КАК ИХ СРАВНИВАТЬ

        // ВЕКТОР ТЕГОВ ДОЛЖЕН 1:ОТОБРОЖАТЬ ВСЕ СЛОВА ОДИНАКОВО 2:  РАНЖИРОВАТЬ ОДИНАКОВО. 3:если отображение встреч 2жды оставляет (ПОКА что) только 1
        // СЕЙЧАС СРАВНИВАЕТ  ТЕГИ У ОДНОГО ВЕКТОРА  И CalculateTagsSimilarity(L,R) != CalculateTagsSimilarity(R,L) думать нужно ли это
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
            int attenuation = maxDistance * distanceAttenuationRate;
            double a = Math.Log(maxDistance,maxDistance+attenuation);
            double rate = distance / Math.Pow(distance+attenuation,a);
            return rate;

        }
    }

}
