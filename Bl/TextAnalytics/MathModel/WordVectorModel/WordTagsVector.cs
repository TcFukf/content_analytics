using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Internal;
using social_analytics.Bl.TextAnalytics.MathModel.Scales;
using Telegram.Td.Api;

namespace social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel
{
    public class WordTagsVector:IEnumerable<KeyValuePair<string,Tag>>
    {
        private Dictionary<string, Tag> _tagsWeights = new();
        public int Length => _tagsWeights.Count;

        // представляет cлово как набор отсортированных слов , отображенных на слова в частотном словаре**
        // (groupA,n*weight(groupA),weight(groupA))
        public WordTagsVector(ITagScales frequenctDictScaler,int numberOfBest ,params string[] words)
        {
            int distance = -1;
            // A1, A2,A3  B C
            _tagsWeights =  new();
            foreach (var tagKey in words.Select(word=>frequenctDictScaler.GetScalse(word)).OrderBy(tag=>frequenctDictScaler.CalcOrder(tag))      )  
            {
                distance++;
                if (distance >= numberOfBest)
                {
                    return;
                }
                double rate = TextAnalyticsTools.SlowDecreasingRate(distance,words.Length);
                double groupWeight = frequenctDictScaler.CalcWeight(tagKey);
                if (groupWeight == 0)
                {
                    continue;
                }
                double weight = groupWeight * rate;
                if (_tagsWeights.ContainsKey(tagKey))
                {
                    _tagsWeights[tagKey].TagWeight += weight;
                }
                else
                {
                    _tagsWeights.Add(tagKey, new Tag() { TagWeight = weight,GroupTagWeight = groupWeight});
                }
            }
        }
        public Tag? GetByKey(string tagKey)
        {
            if (_tagsWeights.ContainsKey(tagKey))
            {
                return _tagsWeights[tagKey];
            }
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_tagsWeights).GetEnumerator();
        }
        IEnumerator<KeyValuePair<string, Tag>> IEnumerable<KeyValuePair<string, Tag>>.GetEnumerator()
        {
            return _tagsWeights.GetEnumerator();
        }
    }
}
