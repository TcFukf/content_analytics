using social_analytics.Bl.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace social_analytics
{
    public class Testing
    {

        //https://glasnarod.ru/specoperaciya/bitva-za-chasov-yar-obstanovka-po-sostoyaniju-na-08-00-5-aprelya-2024-goda/
        public static string text = "Граждан Таджикистана по нескольку часов держат в зонах временного содержания в аэропортах из-за долгих проверок документов. В ведомстве обеспокоены тем, что на территории России нарушаются права и свободы их граждан. Как на это могут отреагировать в Москве и что об этом рассказывают сами граждане Таджикистана";
        public static string text2 = "Отмечается, что в отношении еще 117 граждан Таджикистана принято решение о депортации.\r\n\r\n\"В настоящее время данные граждане также содержатся в бесчеловечных условиях в зонах временного содержания аэропорта Внуково и ожидают отправки на Родину\", - говорится в сообщении. Издание со ссылкой на источник добавляет, что схожая сложная ситуация наблюдается также в аэропортах Шереметьево, Жуковский и Домодедово.\r\n\r\nРанее министерство иностранных дел Таджикистана сообщило о состоявшемся по просьбе российской стороны телефонном разговоре глав МИД Таджикистана и России Сироджиддина Мухриддина и Сергея Лаврова.\r\n\r\n\"Особо подчёркнуты временный характер принятых мер, их ненаправленность против конкретной нации или религии, постепенная нормализация обстановки на пунктах пропуска\", - отметили в МИД России.";
        public static string text3 = "Сегодня мой день рождения и хочу сказать, что ещё год назад у меня было выгорание, я мешкалась и не знала, что мне делать, оставить разработку игр и сменить направление или всё же продолжить изучение. Как видно, я выбрала продолжить и очень рада. \r\n\r\nА ещё год назад я только начинала свой путь, бросила ВУЗ и вместе него взяла курс и начала свое обучение. \r\n\r\nСейчас я уже работаю, не так давно прошла испытательный срок и считаю себя счастливым человеком. Спасибо каждому кто со мной рядом и когда-то был \U0001f970\r\n\r\n27 — это клуб, кому надо, те войдут😎";

        // у каждого слова смотрим по 2 слева и 2 справа, , если n слов в тексте получаем n entityWord где у каждой сущности 4 "соседа" (по 2 у крайних)
        // проходимся по словарю сущностей (Dict<string Word, List<entityWord>>)
        public static List<string> FindEntityGroups(int maxGroupLength,params string[] matches)
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
