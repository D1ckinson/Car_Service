using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

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
        private DetailFabrik _detailFabrik = new DetailFabrik(new DetailNames());
        private CarFabrik _carFabrik = new CarFabrik();
        private DetailNames _detailNames = new DetailNames();

        public ActionBuilder(CarService carService) =>
            _carService = carService;

        public Dictionary<string, Action> MainMenuActions =>
            new Dictionary<string, Action>
            {
                {"Отремонтировать машину", RepairCar },
                {"Купить детали", BuyDetails }
            };

        private Dictionary<string, Func<Detail>> BuyMenuActions =>
            new Dictionary<string, Func<Detail>>
            {
                {$"Купить {_detailNames.Engine}", _detailFabrik.CreateEngine},
                {$"Купить {_detailNames.Pendant}", _detailFabrik.CreatePendant},
                {$"Купить {_detailNames.BrakeSystem}", _detailFabrik.CreateBrakeSystem},
            };

        private void RepairCar()
        {
            _carService.TakeCar(_carFabrik.CreateCar());

            Menu repairMenu = new RepairMenu(CreateRepairMenuOptions(), _carService.ReplaceDetail, UpdateRepairInfo);

            repairMenu.Work();

            Renderer.EraseColumnText(_detailNames.NamesQuantity + 1, Renderer.DetailsForRepairCursorPositionY);
        }

        private void UpdateRepairInfo()
        {
            Renderer.EraseColumnText(_detailNames.NamesQuantity + 1, Renderer.DetailsForRepairCursorPositionY);
            Renderer.DrawRepairInfo(_carService.GiveDetailsForRepair());
            Renderer.DrawStorage(_carService.GetStorageInfo());
        }

        private void BuyDetails()
        {
            BuyMenu buyMenu = new BuyMenu(BuyMenuActions, BuyDetail);

            buyMenu.Work();
        }

        private void BuyDetail(Func<Detail> GiveDetail)
        {
            if (GiveDetail.Invoke() == null)
                return;

            if (_carService.IsMoneyEnough(_detailFabrik.Price))
            {
                Detail detail = GiveDetail.Invoke();

                _carService.TakeDetail(detail);
                _carService.PayMoney(_detailFabrik.Price);

                Renderer.DrawText($"Вы купили {detail.Name}.");
                Renderer.DrawStorage(_carService.GetStorageInfo());
            }
            else
            {
                Renderer.DrawText("У вас недостаточно денег.");
            }
        }

        private Dictionary<string, string> CreateRepairMenuOptions()
        {
            Dictionary<string, string> repairMenuOptions = new Dictionary<string, string>();

            _carFabrik.CarDetails.
                ForEach(detail => repairMenuOptions.Add($"Заменить {detail.Name}.", detail.Name));

            return repairMenuOptions;
        }
    }
}
