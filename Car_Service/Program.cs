using System;
using System.Collections.Generic;
using System.Linq;


namespace Car_Service
{
    internal class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            DetailNames detailNames = new DetailNames();
            DetailFabrik detailFabrik = new DetailFabrik();
            CarService carService = new CarService(detailNames.GiveAllNames());
            ActionBuilder actionBuilder = new ActionBuilder(carService, detailFabrik);
            Menu menu = new MainMenu(actionBuilder.MainMenuActions);

            Renderer.DrawStorage(carService.GetStorageInfo());

            menu.Work();
        }
    }

    abstract class Detail
    {
        protected Detail(string name)
        {
            Name = name;
            IsWorking = true;
        }

        public bool IsWorking { get; protected set; }
        public string Name { get; protected set; }

        public void SetIsWorkingFalse() =>
            IsWorking = false;
    }

    class Pendant : Detail
    {
        public Pendant(string name) : base(name) { }
    }

    class Engine : Detail
    {
        public Engine(string name) : base(name) { }
    }

    class BrakeSystem : Detail
    {
        public BrakeSystem(string name) : base(name) { }
    }

    class ActionBuilder
    {
        private CarService _carService;
        private DetailFabrik _detailFabrik;
        private CarFabrik _carFabrik;
        private DetailNames _detailNames = new DetailNames();

        public ActionBuilder(CarService carService, DetailFabrik detailFabrik)
        {
            _carService = carService;
            _detailFabrik = detailFabrik;
            _carFabrik = new CarFabrik(detailFabrik.AllDetails);
        }

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

    class CarFabrik
    {
        public CarFabrik(List<Detail> details) =>
            CarDetails = details;

        public List<Detail> CarDetails { get; private set; }

        public Car CreateCar()
        {
            List<Detail> carDetails = this.CarDetails;

            BreakCarDetails(carDetails);

            return new Car(carDetails);
        }

        private void BreakCarDetails(List<Detail> details)
        {
            int breakQuantity = RandomUtility.Next(details.Count);

            if (breakQuantity > 0)
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

    class CarService
    {
        private List<Detail> _storage = new List<Detail>();
        private int _money = 10000;
        private int _moneyForRepair = 1500;
        private int _moneyForFailure = 2000;
        private Car _car;
        private List<string> _detailNames;

        public CarService(List<string> detailNames) =>
            _detailNames = detailNames;

        public bool IsMoneyEnough(int price) =>
            _money > price;

        public string[] GetStorageInfo()
        {
            int uniqueDetailsQuantity = _detailNames.Count;
            string[] info = new string[uniqueDetailsQuantity + 1];

            info[0] = $"Ваш капитал = {_money}";
            info[1] = "Количество деталей на складе:";

            for (int i = 1; i < uniqueDetailsQuantity + 1; i++)
            {
                string detailName = _detailNames[i - 1];
                int detailsQuantity = _storage.Count(detail => detail.Name == detailName);

                info[i] = $"{detailName}: {detailsQuantity}";
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

    class DetailFabrik
    {
        public DetailFabrik() =>
            NamesQuantity = GiveAllNames().Count();

        public string EngineName => "Двигатель";
        public string BrakeSystemName => "Тормозная система";
        public string PendantName => "Подвеска";
        public List<Detail> AllDetails =>
            new List<Detail> { CreatePendant(), CreateEngine(), CreateBrakeSystem() };

        public int Price => 1000;
        public int NamesQuantity { get; private set; }

        public List<string> GiveAllNames() =>
            new List<string> { EngineName, BrakeSystemName, PendantName };

        public Engine CreateEngine() =>
            new Engine(EngineName);

        public BrakeSystem CreateBrakeSystem() =>
            new BrakeSystem(BrakeSystemName);

        public Pendant CreatePendant() =>
            new Pendant(PendantName);
    }

    class DetailNames
    {
        public DetailNames() =>
            NamesQuantity = 3;

        public int NamesQuantity { get; private set; }

        public string Engine => "Двигатель";
        public string BrakeSystem => "Тормозная система";
        public string Pendant => "Подвеска";

        public List<string> GiveAllNames() =>
            new List<string> { Engine, BrakeSystem, Pendant };
    }

    abstract class Menu
    {
        private const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        private const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        private const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        protected string[] _items;
        protected int _itemIndex = 0;

        private bool _isRunning;

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                Renderer.DrawMenu(_items, _itemIndex);

                ReadKey();
            }
        }

        protected virtual void ConfirmActionSelection()
        {
            Renderer.EraseColumnText(_items.Length);
            Renderer.EraseText();
        }

        protected void Exit() =>
            _isRunning = false;

        private void SetItemIndex(int index)
        {
            int lastIndex = _items.Length - 1;

            if (index > lastIndex)
                index = lastIndex;

            if (index < 0)
                index = 0;

            _itemIndex = index;
        }

        private void ReadKey()
        {
            switch (Console.ReadKey(true).Key)
            {
                case MoveSelectionDown:
                    SetItemIndex(_itemIndex + 1);
                    break;

                case MoveSelectionUp:
                    SetItemIndex(_itemIndex - 1);
                    break;

                case ConfirmSelection:
                    ConfirmActionSelection();
                    break;
            }
        }
    }

    class MainMenu : Menu
    {
        private Dictionary<string, Action> _actions;

        public MainMenu(Dictionary<string, Action> actions)
        {
            _actions = actions;
            _actions.Add("Выход", Exit);
            _items = _actions.Keys.ToArray();
        }

        protected override void ConfirmActionSelection()
        {
            base.ConfirmActionSelection();

            _actions[_items[_itemIndex]].Invoke();
        }
    }

    class BuyMenu : Menu
    {
        private Dictionary<string, Func<Detail>> _actions;
        private Action<Func<Detail>> _action;

        public BuyMenu(Dictionary<string, Func<Detail>> actions, Action<Func<Detail>> action)
        {
            _actions = actions;
            _actions.Add("Закончить покупки", Exit);
            _items = _actions.Keys.ToArray();
            _action = action;
        }

        protected override void ConfirmActionSelection()
        {
            base.ConfirmActionSelection();

            _action(_actions[_items[_itemIndex]]);
        }

        private new Detail Exit()
        {
            base.Exit();

            return null;
        }
    }

    class RepairMenu : Menu
    {
        private string _exitWord = "Закончить ремонт";
        private Action<string> _action;
        private Action _updateInfo;
        private Dictionary<string, string> _repairOptions;

        public RepairMenu(Dictionary<string, string> repairOptions, Action<string> action, Action updateInfo)
        {
            _action = action;
            _repairOptions = repairOptions;
            _updateInfo = updateInfo;

            _repairOptions.Add(_exitWord, null);
            _items = _repairOptions.Keys.ToArray();

            _updateInfo.Invoke();
        }

        protected override void ConfirmActionSelection()
        {
            base.ConfirmActionSelection();

            if (_items[_itemIndex] == _exitWord)
            {
                Exit();

                return;
            }

            string detailName = _repairOptions[_items[_itemIndex]];

            _action(detailName);

            _updateInfo.Invoke();
        }
    }

    static class RandomUtility
    {
        private static Random s_random = new Random();

        public static int Next(int maxValue) =>
            s_random.Next(maxValue);

        public static int Next(int minValue, int maxValue) =>
            s_random.Next(minValue, maxValue);
    }

    static class Renderer
    {
        public const int DetailsForRepairCursorPositionY = 7;
        private const int TextCursorPositionY = 5;
        private const int StorageCursorPositionY = 12;

        private const ConsoleColor BackgroundColor = ConsoleColor.White;
        private const ConsoleColor ForegroundColor = ConsoleColor.Black;

        private const int SpaceLineSize = 100;
        private const char SpaceChar = ' ';

        public static void DrawStorage(string[] storageInfo)
        {
            for (int i = 0; i < storageInfo.Length; i++)
                DrawText(storageInfo[i], StorageCursorPositionY + i);
        }

        public static void DrawRepairInfo(List<string> detailsName)
        {
            Console.SetCursorPosition(0, DetailsForRepairCursorPositionY);

            Console.WriteLine("Необходимо отремонтировать:");

            detailsName.ForEach(name => Console.WriteLine(name));
        }

        public static void DrawMenu(string[] items, int index)
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < items.Length; i++)
                if (i == index)
                    WriteColoredText(items[i]);
                else
                    Console.WriteLine(items[i]);
        }

        public static void EraseColumnText(int value, int cursorPositionY = 0)
        {
            Console.SetCursorPosition(0, cursorPositionY);

            for (int i = 0; i < value; i++)
                EraseText(cursorPositionY + i);
        }

        public static void DrawText(string text, int cursorPositionY = TextCursorPositionY)
        {
            EraseText(cursorPositionY);

            Console.Write(text);
        }

        public static void EraseText(int cursorPositionY = TextCursorPositionY)
        {
            Console.SetCursorPosition(0, cursorPositionY);
            Console.Write(new string(SpaceChar, SpaceLineSize));
            Console.CursorLeft = 0;
        }

        public static void WriteColoredText(string text, ConsoleColor frontColor = ForegroundColor, ConsoleColor backColor = BackgroundColor)
        {
            Console.ForegroundColor = frontColor;
            Console.BackgroundColor = backColor;

            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
