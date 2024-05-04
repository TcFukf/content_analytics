using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace social_analytics.Bl.structures
{
    public class FrequencyDictionary<Tkey> : IFrequencyDictionary<Tkey>, IHavePlustOperation<FrequencyDictionary<Tkey>>
    {
        public int TotalCount { get; private set; } = 0;
        private Dictionary<Tkey, int> _dict;
        public FrequencyDictionary()
        {
            _dict = new();
            var a = _dict.GetEnumerator();
        }
        public FrequencyDictionary(Dictionary<Tkey,int> dictFreq)
        {
            _dict = dictFreq;
            TotalCount = dictFreq.Values.Sum();
        }

        public int GetKeyCount(Tkey key)
        {
            if (_dict.ContainsKey(key))
            {
                return _dict[key];
            }
            return 0;
        }
        /// <summary>
        /// also meaning - probability to find key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetKeyFrequency(Tkey key)
        {
            if (_dict.ContainsKey(key))
            {
                return _dict[key] / (double)TotalCount ;
            }
            return 0;
        }

        public Tkey[] FindMoreThanAverageKeys()
        {
            int sum = 0;
            foreach (var count in _dict.Values)
            {
                sum += count;
            }
            float average = sum / _dict.Count();
            return _dict.Where(k => k.Value > average + 1).Select(k => k.Key).ToArray();
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
            TotalCount += increment;
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
                    _dict[pair.Key] += pair.Value;
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

        public int Sum()
        {
            return _dict.Values.Sum();
        }
        public string GetJsonDict()
        {
            return JsonConvert.SerializeObject(_dict,Formatting.Indented);
        }

       
    }
}
