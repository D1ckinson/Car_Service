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
    class ActionBuilder
    {
        private CarService _carService;
        private DetailFabrik _detailFabrik;
        private CarFabrik _carFabrik = new CarFabrik();

        public ActionBuilder(CarService carService, DetailFabrik detailFabrik)
        {
            _carService = carService;
            _detailFabrik = detailFabrik;
        }

        public Dictionary<string, Action> GiveMenuActions() =>
            new Dictionary<string, Action>
            {
                {"Отремонтировать машину", RepairCar },
                {"Купить детали", BuyDetails }
            };

        private void RepairCar()
        {
            _carService.TakeCar(_carFabrik.CreateCar());

            Menu repairMenu = new RepairMenu(CreateRepairMenuOptions(), _carService.ReplaceDetail);

            repairMenu.Work();
        }

        private void BuyDetails()
        {

        }

        private Dictionary<string, Type> CreateRepairMenuOptions()
        {
            Dictionary<string, Type> repairMenuOptions = new Dictionary<string, Type>();

            _carFabrik.CarDetailsTypes.
                ForEach(detailType => repairMenuOptions.Add($"Заменить {_detailFabrik.GiveDetailNameByType(detailType)}.", detailType));

            return repairMenuOptions;
        }
    }
}
