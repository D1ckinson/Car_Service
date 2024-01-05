using System;
using System.Collections.Generic;
using System.Linq;

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
    class CarFabrik
    {
        private DetailFabrik _detailFabrik = new DetailFabrik();

        public List<Type> CarDetailsTypes =>
            new List<Type> { _detailFabrik.PendantType, _detailFabrik.EngineType, _detailFabrik.BrakeSystemType };

        public Car CreateCar() =>
            new Car(CreateDetails());

        private Dictionary<Type, Detail> CreateDetails()
        {
            Dictionary<Type, Detail> details = new Dictionary<Type, Detail>();

            CarDetailsTypes.ForEach(type => details.Add(type, _detailFabrik.CreateDetailByType(type)));

            BreakRandomDetail(details);

            return details;
        }

        private void BreakRandomDetail(Dictionary<Type, Detail> details)
        {
            int detailIndex = RandomUtility.Next(CarDetailsTypes.Count);

            Type breakDetailType = CarDetailsTypes[detailIndex];

            details[breakDetailType].SetIsWorkingFalse();
        }
    }
}
