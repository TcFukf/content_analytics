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
            return Regex.Matches(input, @"[\w]+").Where(m=>m.Value.Length>1).Select(match=>match.Value);
        }
        static public void UpdateFrequencyDict<T>(IFrequencyDictionary<T> inputFrequency, params T[] values)
        {
            foreach (var input in values)
            {
                inputFrequency.IncreaseFrequency(input,1);
            }
        }
        static public List<FrequencyGraph<T>> GetGpaphs<T>( int neighborsFromSideCount, params T[] values)
        // if the values have distance <= eighborsFromSide, we consider them neighboring graphs
        {
            // 1 2 3 4 5 6 7 8
           List<FrequencyGraph<T>> graphList = new();
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

                IFrequencyDictionary<T> neight = new FrequencyDictionary<T>();
                foreach (var index in left)
                {
                    neight.IncreaseFrequency(values[index],1);
                }
                foreach (var index in right)
                {
                    neight.IncreaseFrequency(values[index], 1);
                }

                graphList.Add( new FrequencyGraph<T>(values[currentIndex],neight) );

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
            return graphList;
        }
    }
}
