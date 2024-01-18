using System;
using System.Collections.Generic;

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
    class Car
    {
        private List<Detail> _details;

        public Car(List<Detail> details) =>
            _details = details;

        public List<string> GiveDetailsForRepair()
        {
            List<string> detailsNames = new List<string>();

            foreach (Detail detail in _details)
                if (detail.IsWorking == false)
                    detailsNames.Add(detail.Name);

            return detailsNames;
        }

        public void InstallDetail(Detail detail)
        {
            Detail replacementDetail = _details.Find(desiredDetail => desiredDetail.Name == detail.Name);

            _details.Remove(replacementDetail);
            _details.Add(detail);
        }
    }
}
