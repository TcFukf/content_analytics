using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl
{
    public class FrequencyDictionary<Tkey> : IFrequencyDictionary<Tkey>,IHavePlustOperation<FrequencyDictionary<Tkey>>
    {
        private Dictionary<Tkey, int> _dict;
        public FrequencyDictionary()
        {
            _dict = new();
            var a = _dict.GetEnumerator();
        }


        public int? GetFrequency(Tkey key)
        {
            if (_dict.ContainsKey(key))
            {
                return _dict[key];
            }
            return null;
        }

        public Tkey[] FindMoreThanAverageKeys()
        {
            int sum = 0;
            foreach (var count in _dict.Values)
            {
                sum += count;
            }
            float average = sum / _dict.Count();
            return _dict.Where(k=>k.Value>average + 1).Select(k=>k.Key).ToArray();
        }

        public void AddFrequency(Tkey key, int increment)
        {
            if (_dict.ContainsKey(key))
            {
                _dict[key] += increment;
            }
            else
            {
                _dict[key] = increment;
            }
        }
        public override string ToString()
        {
            StringBuilder str = new();
            foreach (var key in _dict.Keys)
            {
                str.Append(key + ":" + _dict[key]);
            }
            return str.ToString();
        }

        public IEnumerator<KeyValuePair<Tkey, int>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<Tkey, int>>)_dict).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }

        public FrequencyDictionary<Tkey> Plus(FrequencyDictionary<Tkey> add)
        {
            foreach (var pair in add)
            {
                if (_dict.ContainsKey(pair.Key))
                {
                    _dict[pair.Key]+=pair.Value;
                }
                else
                {
                    _dict[pair.Key] = pair.Value;
                }
            }
            return this;
        }

        public FrequencyDictionary<Tkey> Value()
        {
            return this;
        }
    }
}
