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
        private double _groupSumTagsOfVector;
        public double GroupSumTagsOfVector { get; private set; }
        public int Length => _tagsWeights.Count;

        // много чего оптимизироваьт мона .. если веса очень маленькие можно скипать
        public WordTagsVector(ITagScales frequenctDictScaler,int maxLength ,params string[] words)
        {
            int distance = -1;
            // A1, A2,A3  B C
            _tagsWeights =  new();
            foreach (var tagKey in words.Select(word=>frequenctDictScaler.GetScalse(word)).OrderBy(tag=>frequenctDictScaler.CalcOrder(tag))      )  
            {
                distance++;
                if (distance >= maxLength)
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
                    GroupSumTagsOfVector += groupWeight;
                }
                if (GroupSumTagsOfVector != 0 && weight / GroupSumTagsOfVector < 0.1d / 100)
                {
                    break;
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
        public IEnumerable<string> GetTagKeys()
        {
            return _tagsWeights.Keys;
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
