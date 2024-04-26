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
        public static string text2 = "Верховная Рада приняла закон, согласно которому будут увольнять украинцев, чьи родственники проживают в новых регионах России\r\n\r\nОчередной украинский законопроект от Верховной Рады побил (https://t.me/readovkanews/78361) еще один рекорд по притеснению собственных граждан. Согласно ему, работодатель может уволить своего подчиненного, если он не сообщил ему о родственниках или близких, который остались жить на освобожденных территориях.\r\n\r\nЧто интересно, сам закон трактуется как «невыполнение работником правил поведения на предприятии или в учреждении». По сути, увольнять будут не просто за какие-то дисциплинарные нарушения, а именно за утаивание родственных связей. Не исключено, что законопроект станут применять в адрес мужчин призывного возраста, имеющих ту или иную «бронь» от мобилизации, (https://t.me/readovkanews/78715) а на их место работодатели предпочтут брать уже женщин.";
        public static string text = "Европол сообщил о задержании подозреваемых в краже редких книг из европейских библиотек. Часть книг были проданы на аукционах в России\r\n\r\nВ четырех странах Евросоюза и Грузии были задержаны девять подозреваемых в краже не менее 170 редких и антикварных книг из европейских библиотек. Об этом говорится (https://www.europol.europa.eu/media-press/newsroom/news/9-georgian-nationals-arrested-for-stealing-antique-books) в сообщении на сайте Европола.\r\n\r\nВсе задержанные — граждане Грузии, пишет ведомство. По версии следствия, в 2022–2023 годах подозреваемые похитили книги из национальных и исторических библиотек девяти стран Евросоюза. Их основной целью были редкие книги русских писателей, в частности, первые издания книг Пушкина и Гоголя.\r\n\r\nВ библиотеках подозреваемые просили выдать им редкие книги, притворяясь, что у них есть «академический интерес». После этого они фотографировали издания, возвращались через несколько дней и подменяли настоящие книги подделками. По данным экспертов, это были копии «превосходного качества».\r\n\r\nВ других случаях подозреваемые прибегали к «более грубому подходу» и просто «врывались в библиотеки», пишет Европол.\r\n\r\nСогласно сообщению ведомства, некоторые из украденных книг были проданы на аукционах в Санкт-Петербурге и Москве. Общий финансовый ущерб от краж составил около 2,5 млн евро.\r\n";
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
