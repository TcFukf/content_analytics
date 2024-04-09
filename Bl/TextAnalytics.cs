using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelegramWrapper.Models;

namespace social_analytics.Bl
{
    public class TextAnalytics
    {
        static public IEnumerable<string> GetStringEntities(string input)
        {
            return Regex.Matches(input, @"[\w]+").Where(m=>m.Value.Length>1).Select(match=>match.Value.ToLower());
        }
        static public void UpdateFrequencyDict<T>(IFrequencyDictionary<T> inputFrequency, params T[] values)
        {
            foreach (var input in values)
            {
                inputFrequency.AddFrequency(input,1);
            }
        }
        static public IEnumerable<FrequencyGraph<T>> GetGpaphs<T>( int neighborsFromSideCount, params T[] values)
        // if the values have distance <= eighborsFromSide, we consider them neighboring graphs
        {
            // 1 2 3 4 5 6 7 8
           Dictionary<T,FrequencyGraph<T>> graphList = new();
            Queue<int> right = new(neighborsFromSideCount);
            Queue<int> left = new(neighborsFromSideCount);
            int currentIndex = 0;
            if (currentIndex + 1 < values.Length)
            {
                right.Enqueue(currentIndex + 1);
            }
            for (currentIndex = 0; currentIndex < values.Length; currentIndex++)
            {
                if (currentIndex + neighborsFromSideCount < values.Length)
                {
                    right.Enqueue(currentIndex + neighborsFromSideCount);
                }

                FrequencyDictionary<T> neight = new FrequencyDictionary<T>();
                foreach (var index in left)
                {
                    neight.AddFrequency(values[index],1);
                }
                foreach (var index in right)
                {
                    neight.AddFrequency(values[index], 1);
                }
                if (graphList.ContainsKey(values[currentIndex]))
                {
                    graphList[values[currentIndex]].Neightboors = neight.Plus(graphList[values[currentIndex]].Neightboors as FrequencyDictionary<T>);
                }
                else
                {
                    graphList.Add(values[currentIndex], new FrequencyGraph<T>(values[currentIndex],neight) );
                }

                if (left.Count() > 1)
                {
                    left.Dequeue();
                }
                left.Enqueue(currentIndex);

                if (right.Count() > 0)
                {
                    right.Dequeue();
                }

            }
            return graphList.Values;
        }
    }

}
