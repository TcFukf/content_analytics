using social_analytics.Bl.structures;
using social_analytics.Bl.TextAnalytics.MathModel.WordTransformers;
using social_analytics.Bl.TextAnalytics.MathModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace social_analytics.Bl.TextAnalytics.TextFiles
{
    public static class ParsBigFile
    {
        public static void CreateFreqDictionarieFromFile(string inputFilePath,string outputFileFullPath)
        {
            FrequencySkye bookSkye = new FrequencySkye(new FrequencyDictionary<string>(), new PorterTransformator());
                if (new string[] { ".txt", ".json", ".csv" }.Contains(Path.GetExtension(inputFilePath)))
                {
                    foreach (var lines in TextAnalyticsTools.GetStringsFromFileByLines(inputFilePath))
                    {
                        bookSkye.UpdateFrequencies(TextAnalyticsTools.GetStringEntities(lines.ToArray()));
                    }
                    bookSkye.SaveWordsInFile(outputFileFullPath);
                }

        }
        public static long FileLinesCount(string fullFilePath)
        {
            long linesCount = 0;
            using (StreamReader sr = new(fullFilePath, TextFiles.EncodingDetector.DetectFileEncoding(fullFilePath)))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    linesCount++;
                    line = sr.ReadLine();
                }
            }
            return linesCount;
        }
        /// <summary>
        /// merge ferquencyDict with json file/
        /// set freqDict new merged dict;
        /// </summary>
        public static IEnumerable<Dictionary<string,int>> ReadFreqDictFromJson(string filePath, int bufferLines = 1_000)
        {
            string startOfJsonSequency = "{";
            string endOfJsonSequency = "}";
            List<string> buffer = new List<string>(bufferLines);
            using (StreamReader sr = new(filePath, TextFiles.EncodingDetector.DetectFileEncoding(filePath)) )
            {
                string line = sr.ReadLine();
                if (line == startOfJsonSequency)
                {
                    line = sr.ReadLine();
                }
                while(line!=null && line != endOfJsonSequency)
                {
                    buffer.Add(line);
                    if (buffer.Count >= bufferLines)
                    {
                        var dictValues = JsonConvert.DeserializeObject<Dictionary<string, int>>($"{{{string.Join(" ",buffer)}}}" );
                        yield return dictValues;
                        dictValues = null;
                        buffer.Clear();
                    }
                    line = sr.ReadLine();
                }
                if (buffer.Count > 0)
                {
                    string serString ;
                    if (buffer.First() == buffer.Last()  && buffer.First() == "{}")
                    {
                        serString = string.Join(" ", buffer);
                    }
                    else
                    {
                        serString = $"{{{string.Join(" ", buffer)}}}";
                    }
                var dictValues = JsonConvert.DeserializeObject<Dictionary<string, int>>(serString);
                    yield return dictValues;
                }
            }
        }
        public static string[] ParteBigFile(string fullFilePath,string dirPathOut,int linesPartCount = 16500000)
        {
            long linesCount = 0;
            long partSizeCoint = linesPartCount;
            List<string> names = new List<string>();
            string currentName = Path.GetFileNameWithoutExtension(fullFilePath);
            int prefix = 10000;
            string exten = ".txt";
            names.Add(dirPathOut+currentName + prefix + exten);
            using (StreamReader sr = new(fullFilePath, TextFiles.EncodingDetector.DetectFileEncoding(fullFilePath)))
            {
                    var fs = new StreamWriter(dirPathOut + currentName + prefix + exten);
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        linesCount++;
                        fs.WriteLine(line);
                        line = sr.ReadLine();
                        if (linesCount >= partSizeCoint)
                        {
                            linesCount = 0;
                            fs.Close();
                            prefix++;
                            fs = new StreamWriter(dirPathOut + currentName + prefix + exten);
                            names.Add(dirPathOut + currentName + prefix + exten);
                        }
                    }
                fs.Close();
            }
            return names.ToArray();
            
        }
     }

}

