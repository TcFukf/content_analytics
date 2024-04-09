using social_analytics.Bl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace social_analytics
{
    public class TrySomething
    {

        //https://glasnarod.ru/specoperaciya/bitva-za-chasov-yar-obstanovka-po-sostoyaniju-na-08-00-5-aprelya-2024-goda/
        public static string text = "Битва за Часов Яр: рывок ВС РФ к микрорайону «Канал» обстановка по состоянию на 08.00 5 апреля 2024 года." +
            " На Бахмутском направлении российские войска развивают наступление в направлении Часов Яра.По предварительной информации," +
            " подразделениям ВДВ удалось выйти через урочище Ступки к окраинам канала Северский Донец — Донбасс." +
            "Позднее появились сведения о достижении ВС РФ окраин микрорайона «Канал»: бойцы 98 воздушно-десантной дивизии зашли на окраины населенного пункта через лес со стороны трассы О0506" +
            " и проломили оборону украинских формирований на этом участке. Появившиеся кадры объективного контроля со стороны противника подтверждают факт атаки российских войск и их зацепление" +
            " за окраины микрорайона на улице Зеленой.А с учетом того, что медиаресурсы ВСУ обычно выкладывают более выгодные для себя " +
            "материалы, реальное продвижение ВС РФ может быть даже еще больше, сообщает ТК Рыбарь";
        // у каждого слова смотрим по 2 слева и 2 справа, , если n слов в тексте получаем n entityWord где у каждой сущности 4 "соседа" (по 2 у крайних)
        // проходимся по словарю сущностей (Dict<string Word, List<entityWord>>)
        public List<string> FindEntityGroups(int maxGroupLength,params string[] matches)
        {
            Dictionary<string, IFrequencyDictionary<string>> freqs = new();
            Queue<string> group = new Queue<string>();
            int currentInputIndex;
            for (currentInputIndex = 0; currentInputIndex < maxGroupLength; currentInputIndex++)
            {
                if ( currentInputIndex < matches.Count())
                {
                    group.Enqueue(matches[currentInputIndex]);
                }
            }
            while( currentInputIndex < matches.Count())
            {

                var arr = group.ToArray();
                for (int wordIndex = 0; wordIndex < arr.Length; wordIndex++)
                {
                    string word = arr[wordIndex];
                    if (freqs.ContainsKey(word) == false)
                    {
                        freqs[word] = new FrequencyDictionary<string>();
                    }

                    for (int j = 0; j < arr.Length; j++)
                    {
                        if (wordIndex != j)
                        {
                            freqs[word].AddFrequency(arr[j], 1);
                        }
                    }
                }
                group.Dequeue();
                group.Enqueue(matches[currentInputIndex]);
                currentInputIndex++;
            }
            List<string> mb = new();
            HashSet<string> seen = new();
            foreach (var entity in freqs)
            {
                var canBe = freqs[entity.Key].FindMoreThanAverageKeys().Where(word=>!seen.Contains(word));
                if (canBe.Count() > 0)
                {
                string add = string.Join(", ",canBe);
                mb.Add(entity.Key+" "+add);
                }
                seen.Add(entity.Key) ;
            }
            return mb;
        }
    }
}
