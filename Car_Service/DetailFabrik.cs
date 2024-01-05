﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

//У вас есть автосервис, в который приезжают люди, чтобы починить свои автомобили.
//У вашего автосервиса есть баланс денег и склад деталей.
//Когда приезжает автомобиль, у него сразу ясна его поломка,
//и эта поломка отображается у вас в консоли вместе с ценой за починку(цена за починку складывается из цены детали + цена за работу).
//Поломка всегда чинится заменой детали, но количество деталей ограничено тем, что находится на вашем складе деталей.
//Если у вас нет нужной детали на складе, то вы можете отказать клиенту, и в этом случае вам придется выплатить штраф.
//Если вы замените не ту деталь, то вам придется возместить ущерб клиенту.
//За каждую удачную починку вы получаете выплату за ремонт, которая указана в чек-листе починки.
//Класс Деталь не может содержать значение “количество”.
//Деталь всего одна, за количество отвечает тот, кто хранит детали.
//При необходимости можно создать дополнительный класс для конкретной детали и работе с количеством.

namespace Car_Service
{
    class DetailFabrik
    {
        private Dictionary<Type, Func<string, Detail>> _detailByType;
        private Dictionary<Type, string> _detailNameByType;

        public DetailFabrik()
        {
            _detailByType = new Dictionary<Type, Func<string, Detail>>
            {
                { EngineType, CreateEngine },
                { BrakeSystemType, CreateBrakeSystem },
                { PendantType, CreatePendant }
            };

            _detailNameByType = new Dictionary<Type, string>
            {
                { EngineType, "Двигатель" },
                { BrakeSystemType, "Тормозная система" },
                { PendantType, "Подвеска" }
            };
        }

        public Type EngineType => typeof(Engine);
        public Type BrakeSystemType => typeof(BrakeSystem);
        public Type PendantType => typeof(Pendant);

        public string EngineName => "Двигатель";
        public string BrakeSystemName => "Тормозная система";
        public string PendantName => "Подвеска";

        public Detail CreateDetailByType(Type detailType) =>
            _detailByType[detailType].Invoke(_detailNameByType[detailType]);

        public string GiveDetailNameByType(Type detailType) =>
            _detailNameByType[detailType];

        private Engine CreateEngine(string name) =>
            new Engine(name);

        private BrakeSystem CreateBrakeSystem(string name) =>
            new BrakeSystem(name);

        private Pendant CreatePendant(string name) =>
            new Pendant(name);
    }
}