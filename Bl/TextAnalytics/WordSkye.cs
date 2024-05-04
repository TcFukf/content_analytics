using Newtonsoft.Json;
using social_analytics.Bl.structures;
using System.Collections;

namespace social_analytics.Bl.TextAnalytics
{
    public class WordSkye : IFrequencyDictionary<string>
    {
        private IFrequencyDictionary<string> _freqDict;

        public IWordTransforamtor WordTranformator { get; private set; }

        public int TotalCount => _freqDict.TotalCount;

        public WordSkye(IFrequencyDictionary<string> freqDict,IWordTransforamtor wordTransforamtor)
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
        public async Task SaveInFile(string fullPath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            string line = JsonConvert.SerializeObject(_freqDict,settings);
            using (Stream str = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(str))
                {
                    await sw.WriteLineAsync(line);
                }
            }
        }
        public async Task LoadFromFile(string fullPath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            using (Stream str = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader sw = new StreamReader(str))
                {
                    string line = await sw.ReadLineAsync();
                    var freqDict = JsonConvert.DeserializeObject<FrequencyDictionary<string>>(sw.ReadLine(),settings);
                    _freqDict = freqDict;
                }
            }
        }
    }
    
}
