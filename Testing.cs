﻿using social_analytics.Bl.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace social_analytics
{
    public static  class Testing
    {

        //https://glasnarod.ru/specoperaciya/bitva-za-chasov-yar-obstanovka-po-sostoyaniju-na-08-00-5-aprelya-2024-goda/
        public static string text1 = "В качестве исключения будет допустим экспорт российского сахара в страны ЕАЭС в следующих объемах: в Казахстан — 120 тыс. тонн; в Киргизию — 28,5 тыс. тонн; в Армению — 28 тыс. тонн; в Беларусь — 5 тыс. тонн.\r\n\r\nВ пояснительной записке к проекту документа, определяющего объемы вывозимого сахара, указано, что запрет вводится в целях обеспечения сбалансированности внутреннего рынка сахара в рамках ЕАЭС, а также для сдерживания роста потребительских цен на социально-значимую продукцию.\r\n\r\nМинсельхоз разработал проект постановления о временном запрете экспорта сахара в марте 2024 года. В апреле временный запрет на экспорт сахара поддержала подкомиссия по таможенно-тарифному и нетарифному регулированию, защитным мерам во внешней торговле, писал Интерфакс со ссылкой на источники на рынке. По данным собеседников агентства, обсуждение запрета состоялось в конце февраля.\r\n\r\nВ качестве причины для такого решения источники называли «беспокойство по поводу активизации поставок сахара за рубеж», так как к новому сезону необходимо подготовить достаточные переходящие запасы.\r\n\r\nВ середине марта Минсельхоз также отчитался об экспорте 700 тыс. тонн сахара с 1 августа по конец февраля 2024 года. В ведомстве подчеркнули, что это показатель в 3,3 раза превышает аналогичный период прошлого сезона. Министерство также отметило, что до конца сезона 2023/2024 на экспорт может быть определено около 200 тыс. тонн сахара, и предложило направить этот объем в страны ЕАЭС.\r\n\r\n«Предлагаемая Минсельхозом России мера регулирования позволит полностью исполнить договоренности по поставкам сахара нашим партнерам по ЕАЭС и обеспечить стабильную ситуацию на внутреннем рынке», — заявили в Минсельхозе.";
        public static string text2 = "Ранее источники \"Интерфакса\" сообщали, что вопрос о запрете экспорта сахара из РФ обсуждался в Минсельхозе еще в конце февраля. Необходимость этой меры аргументировалась \"беспокойством по поводу активизации поставок сахара за рубеж\". \"В этой ситуации важно, чтобы к новому сезону были достаточные переходящие запасы\", - говорил один из источников.\r\n\r\nМинсельхоз в середине марта сообщал, что Россия с 1 августа 2023 года по конец февраля 2024 года экспортировала более 700 тыс. тонн сахара, что в 3,3 раза больше, чем за аналогичный период прошлого сезона (сельхозгод по сахару стартует в РФ с 1 августа - ИФ). Ограничение экспорта позволит сохранить достаточный объем переходящих запасов и обеспечить сахаром партнеров по ЕАЭС, поясняло ведомство, комментируя свое предложение о введении запрета на экспорт сахара до 31 августа 2024 года.\r\n\r\nМинсельхоз отмечал, что российский сахар является одним из самых доступных в мире, что стимулирует спрос на него на мировом рынке. В этом сезоне заметно расширилась география поставок отечественного сахара. В частности, его импорт начали Туркмения, Афганистан, Турция, КНДР, Сенегал.\r\n\r\n\"С учетом необходимости обеспечения достаточного уровня остатков сахара на конец текущего сезона, а также принимая во внимание прогнозный объем производства и потребности внутреннего рынка в этом продукте, до конца 2023/24 сезона на экспорт может быть направлено порядка 200 тыс. тонн. Исходя из прогнозного индикативного баланса сахара по странам ЕАЭС на 2024 год, данный объем до конца сезона должен быть направлен нашим партнерам в страны ЕАЭС\", - говорилось в сообщении Минсельхоза.";
        public static string text3 = "Москва. 4 мая. INTERFAX.RU - МВД России объявило в розыск президента Украины Владимира Зеленского.\r\n\r\nКак следует из базы розыска министерства, Зеленский, 1978 года рождения, уроженец города Кривой Рог Днепропетровской области Украины, разыскивается по статье УК РФ.\r\n\r\nДругих подробностей в министерстве не привели";
        public static string text4 = "Сегодня мой день рождения и хочу сказать, что ещё год назад у меня было выгорание, я мешкалась и не знала, что мне делать, оставить разработку игр и сменить направление или всё же продолжить изучение. Как видно, я выбрала продолжить и очень рада. \r\n\r\nА ещё год назад я только начинала свой путь, бросила ВУЗ и вместе него взяла курс и начала свое обучение. \r\n\r\nСейчас я уже работаю, не так давно прошла испытательный срок и считаю себя счастливым человеком. Спасибо каждому кто со мной рядом и когда-то был \U0001f970\r\n\r\n27 — это клуб, кому надо, те войдут😎";
        public static string _lines = "ВТБ снизил минимальную ставку по кредитам наличными\r\nСбербанк снизил ставки по ряду кредитов \n16; Правительство внесло изменения в программу развития Курил\r\n18; Кабмин увеличил финансирование федеральной программы развития Курил\r\n19; Правительство увеличило финансирование программы развития Курил\r\n\r\n6; Инженеры стали самыми востребованными на рынке труда\r\n12; Названы самые востребованные в России профессии\r\n20; Стали известны самые востребованные профессии в России\r\n26; Инженеры признаны самыми востребованными на рынке труда в РФ\r\n32; Инженеры стали самыми востребованными на рынке труда РФ в сентябре\r\n51; Названы самые востребованные профессии в России\r\n53; Минтруд назвал самые востребованные профессии в России\r\n\r\n25; Сбербанк с 16 октября снижает ставки по потребительским кредитам\r\n31; Сбербанк снизил процентные ставки по ряду кредитов\r\n37; Сбербанк снизил ставки по ряду кредитов\r\n\r\n0; В России выпустят собственную криптовалюту — крипторубль\r\n5; Россия срочно создает крипторубль\r\n27; В России займутся выпуском крипторубля\r\n35; В России создадут свою криптовалюту\r\n36; Россия начнет выпускать крипторубли\r\n42; Россия выпустит собственную криптовалюту – крипторубль";
        
        public static string[] GetArtickes()
        {
            return _lines.Split("\n").Where(ar=>ar.Length > 2).ToArray();
        }
        public static string[] GetNews()
        {
            return new string[] {text1,text2,text3,text4 };
        }
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
