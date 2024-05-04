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
        public static string text = "Госсекретарь США Энтони Блинкен надеется, что Россия даст согласие на переговоры по урегулированию на Украине в соответствии с Уставом ООН. Об этом он заявил на Всемирном экономическом форуме в Эр-Рияде.\r\n\r\n«Я надеюсь, что... [Россия] продемонстрирует готовность к искренним переговорам в соответствии с основными принципами, лежащими в основе международного сообщества и Устава ООН. Суверенитет, территориальная целостность, независимость. И если они будут надлежащим образом подтверждены, должна быть принята резолюция»,";
        public static string text2 = "20–24 июня Швейцария проведет в Бюргенштоке конференцию по мирному урегулированию на Украине. Участие России в саммите не предусмотрено. Пресс-секретарь президента РФ Дмитрий Песков заявлял, что переговорный процесс по урегулированию конфликта на Украине без России бессмыслен. Официальный представитель МИД РФ Мария Захарова указывала, что Россия не примет участия в конференции по Украине в Швейцарии, даже если это приглашение поступит. Отмечалось, что Украина намерена использовать конференцию для продвижения «формулы мира» президента Владимира Зеленского, содержащей неприемлемые для России пункты, такие как вывод войск за границы Украины 1991 г.";
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
