﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics
{
    internal class TrySomething
    {
        //https://glasnarod.ru/specoperaciya/bitva-za-chasov-yar-obstanovka-po-sostoyaniju-na-08-00-5-aprelya-2024-goda/
        string text = "Битва за Часов Яр: рывок ВС РФ к микрорайону «Канал» обстановка по состоянию на 08.00 5 апреля 2024 года." +
            " На Бахмутском направлении российские войска развивают наступление в направлении Часов Яра.По предварительной информации," +
            " подразделениям ВДВ удалось выйти через урочище Ступки к окраинам канала Северский Донец — Донбасс." +
            "Позднее появились сведения о достижении ВС РФ окраин микрорайона «Канал»: бойцы 98 воздушно-десантной дивизии зашли на окраины населенного пункта через лес со стороны трассы О0506" +
            " и проломили оборону украинских формирований на этом участке. Появившиеся кадры объективного контроля со стороны противника подтверждают факт атаки российских войск и их зацепление" +
            " за окраины микрорайона на улице Зеленой.А с учетом того, что медиаресурсы ВСУ обычно выкладывают более выгодные для себя " +
            "материалы, реальное продвижение ВС РФ может быть даже еще больше, сообщает ТК Рыбарь";
        // у каждого слова смотрим по 2 слева и 2 справа, , если n слов в тексте получаем n entityWord где у каждой сущности 4 "соседа" (по 2 у крайних)
        // проходимся по словарю сущностей (Dict<string Word, List<entityWord>>)
        
    }
}