using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using social_analytics.Bl.TextAnalytics.MathModel.Scales;
using Telegram.Td.Api;

namespace social_analytics.Bl.TextAnalytics.MathModel
{
    public class WordTags : IEnumerable
    {
        public int Length => _tags.Count;
        private List<string> _tags;
        private double _totalSum = 0d;
        private Dictionary<string, int> _hashIndexes = null;
        public WordTags(IEnumerable<string> words, ITagScales scales, bool hashGetByKey)
        {
            _tags = words.OrderBy(word => scales.CalcOrder(word)).DistinctBy(w => w).Select(word => word).ToList();
            if (hashGetByKey)
            {
                HashGetByKey();
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        public double TotalSum(ITagScales scales)
        {
            if (_totalSum == 0)
            {
                for (int i = 0; i < _tags.Count; i++)
                {
                    _totalSum += scales.CalcWeight(_tags[i]);
                }
            }
            return _totalSum;
        }

        public (string tagKey, int tagPosition) GetByIndex(int i)
        {
            string word = _tags[i];
            return (word, i);
        }

        public (string tagKey, int tagPosition)? GetByKey(string tagKey)
        {
            if (_hashIndexes != null && _hashIndexes.ContainsKey(tagKey))
            {
                return (tagKey, _hashIndexes[tagKey]);
            }
            for (int i = 0; i < _tags.Count; i++)
            {
                if (_tags[i] == tagKey)
                {
                    return (tagKey, i);
                }
            }
            return null;
        }

        internal void HashGetByKey()
        {
            if (_hashIndexes == null)
            {
                _hashIndexes = new();
                for (int i = 0; i < _tags.Count; i++)
                {
                    _hashIndexes.Add(_tags[i], i);
                }
            }
        }
    }
}
