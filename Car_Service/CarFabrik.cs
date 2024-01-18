﻿using System;
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
        private DetailFabrik _detailFabrik = new DetailFabrik(new DetailNames());

        public List<Detail> CarDetails => new List<Detail> { _detailFabrik.CreatePendant(), _detailFabrik.CreateEngine(), _detailFabrik.CreateBrakeSystem() };

        public Car CreateCar()
        {
            List<Detail> CarDetails = this.CarDetails;

            BreakCarDetails(CarDetails);

            return new Car(CarDetails);
        }

        private void BreakCarDetails(List<Detail> details)
        {
            int breakQuantity = RandomUtility.Next(details.Count);

            if (breakQuantity == 0)
            {
                int detailIndex = RandomUtility.Next(details.Count);

                details[detailIndex].SetIsWorkingFalse();

                return;
            }

            BreakRandomDetails(details, breakQuantity);
        }

        private void BreakRandomDetails(List<Detail> details, int quantity)
        {
            List<Detail> tempDetails = new List<Detail>(details);

            for (int i = 0; i < quantity; i++)
            {
                int detailIndex = RandomUtility.Next(tempDetails.Count);

                Detail detail = tempDetails[detailIndex];

                detail.SetIsWorkingFalse();

                tempDetails.Remove(detail);
            }
        }
    }
}
