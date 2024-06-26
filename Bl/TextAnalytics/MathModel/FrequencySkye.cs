﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using social_analytics.Bl.structures;
using social_analytics.Bl.TextAnalytics.MathModel.WordTransformers;
using System.Collections;
using System.Runtime.InteropServices.JavaScript;

namespace social_analytics.Bl.TextAnalytics.MathModel
{
    [JsonObject]
    public class FrequencySkye : IFrequencyDictionary<string>
    {
        [JsonProperty]
        private IFrequencyDictionary<string> _freqDict;

        public IWordTransforamtor WordTranformator { get; private set; }

        public long TotalCount => _freqDict?.TotalCount ?? 0;

        public FrequencySkye(IFrequencyDictionary<string> freqDict, IWordTransforamtor wordTransforamtor)
        {
            _freqDict = freqDict;
            WordTranformator = wordTransforamtor;
        }
        public void UpdateFrequencies(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                AddFrequency(word, 1);
            }
        }
        public void AddFrequency(string key, int increment)
        {
            string tranfkey = WordTranformator.TransformWord(key);
            _freqDict.AddFrequency(tranfkey, increment);
        }

        public string[] FindMoreThanAverageKeys()
        {
            return _freqDict.FindMoreThanAverageKeys();
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return _freqDict.GetEnumerator();
        }

        public int GetKeyCount(string key)
        {
            string tranfkey = WordTranformator.TransformWord(key);
            return _freqDict.GetKeyCount(tranfkey);
        }

        public double GetKeyFrequency(string key)
        {
            string tranfkey = WordTranformator.TransformWord(key);
            return _freqDict.GetKeyFrequency(tranfkey);
        }

        public int Sum()
        {
            return _freqDict.Sum();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _freqDict.GetEnumerator();
        }
        public void SaveWordsInFile(string fullFILEPath)
        {
            string line = (_freqDict as FrequencyDictionary<string>).GetJsonDict();
            using (Stream str = new FileStream(fullFILEPath+".json", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(str))
                {
                    sw.Write(line);
                }
            }
        }
        public void LoadWordsFromJsonFile(string fullPath)
        {
            using (Stream str = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader sw = new StreamReader(str))
                {
                    string line = sw.ReadToEnd();
                    var dictValues = JsonConvert.DeserializeObject<Dictionary<string, int>>(line);
                    FrequencyDictionary<string> frequenctDict = new(dictValues);
                    _freqDict = frequenctDict;
                }
            }
        }

        public void Clear()
        {
            _freqDict.Clear();
        }
    }

}
