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
    class CarService
    {
        private List<Detail> _storage = new List<Detail>();
        private int _money = 10000;
        private int _moneyForRepair = 1500;
        private int _moneyForFailure = 2000;
        private Car _car;
        private DetailNames _detailNames;

        public CarService(DetailNames detailNames)
        {
            _detailNames = detailNames;
            _storage.Add(new BrakeSystem(_detailNames.BrakeSystem));
            _storage.Add(new Engine(_detailNames.Engine));
            _storage.Add(new Pendant(_detailNames.Pendant));
        }

        public bool IsMoneyEnough(int price) =>
            _money > price;

        public string[] GetStorageInfo()
        {
            int uniqueDetailsQuantity = _detailNames.NamesQuantity;
            string[] info = new string[uniqueDetailsQuantity + 1];
            List<string> allDetailsNames = _detailNames.GiveAllNames();

            info[0] = $"Ваш капитал = {_money}.";
            info[1] = "Количество деталей на складе:";

            for (int i = 1; i < uniqueDetailsQuantity + 1; i++)
            {
                string detailName = allDetailsNames[i - 1];
                int detailsQuantity = _storage.Count(detail => detail.Name == detailName);

                info[i] = $"{detailName} {detailsQuantity}";
            }

            return info;
        }

        public void TakeCar(Car car) =>
            _car = car;

        public void TakeDetail(Detail detail) =>
            _storage.Add(detail);

        public void PayMoney(int price) =>
            _money -= price;

        public List<string> GiveDetailsForRepair() =>
            _car.GiveDetailsForRepair();

        public void ReplaceDetail(string detailName)
        {
            Detail detail = _storage.FirstOrDefault(detailToFind => detailToFind.Name == detailName);

            if (detail == null)
            {
                Renderer.DrawText($"{detailName} нет на складе.");

                return;
            }

            if (_car.GiveDetailsForRepair().FirstOrDefault(detailsName => detail.Name == detailsName) == null)
            {
                Renderer.DrawText("Вы заменили не ту деталь и вас оштрафовали.");

                _money -= _moneyForFailure;

                if (_money < 0)
                    _money = 0;

                return;
            }

            _car.InstallDetail(detail);
            _storage.Remove(detail);

            Renderer.DrawText($"Вы заменили {detailName}.");

            _money += _moneyForRepair;
        }
    }
}
