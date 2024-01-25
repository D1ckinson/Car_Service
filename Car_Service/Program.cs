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

            Renderer renderer = new Renderer();
            DetailFabrik detailFabrik = new DetailFabrik();
            CarService carService = new CarService(detailFabrik.GiveAllNames(), renderer);
            ActionBuilder actionBuilder = new ActionBuilder(carService, detailFabrik, renderer);
            Menu menu = new MainMenu(actionBuilder.MainMenuActions);

            renderer.DrawStorage(carService.GetStorageInfo());

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

        public void Break() =>
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
        private Renderer _renderer;

        public ActionBuilder(CarService carService, DetailFabrik detailFabrik, Renderer renderer)
        {
            _renderer = renderer;
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
                {$"Купить {_detailFabrik.EngineName}", _detailFabrik.CreateEngine},
                {$"Купить {_detailFabrik.PendantName}", _detailFabrik.CreatePendant},
                {$"Купить {_detailFabrik.BrakeSystemName}", _detailFabrik.CreateBrakeSystem},
            };

        private void RepairCar()
        {
            _carService.TakeCar(_carFabrik.CreateCar());

            Menu repairMenu = new RepairMenu(CreateRepairMenuOptions(), _carService.ReplaceDetail, UpdateRepairInfo);

            repairMenu.Work();

            _renderer.EraseColumnText(_detailFabrik.NamesQuantity + 1);
        }

        private void UpdateRepairInfo()
        {
            _renderer.EraseColumnText(_detailFabrik.NamesQuantity + 1);
            _renderer.DrawRepairInfo(_carService.GiveDetailsForRepair());
            _renderer.DrawStorage(_carService.GetStorageInfo());
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

                _renderer.DrawText($"Вы купили {detail.Name}.");
                _renderer.DrawStorage(_carService.GetStorageInfo());
            }
            else
            {
                _renderer.DrawText("У вас недостаточно денег.");
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
        private List<Detail> _carDetails;

        public CarFabrik(List<Detail> details) =>
            _carDetails = details;

        public List<Detail> CarDetails =>
            new List<Detail>(_carDetails);

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

                details[detailIndex].Break();

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

                detail.Break();

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
        private Renderer _renderer;

        public CarService(List<string> detailNames, Renderer renderer)
        {
            _renderer = renderer;
            _detailNames = detailNames;
        }

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
                _renderer.DrawText($"{detailName} нет на складе.");

                return;
            }

            if (_car.GiveDetailsForRepair().FirstOrDefault(detailsName => detail.Name == detailsName) == null)
            {
                _renderer.DrawText("Вы заменили не ту деталь и вас оштрафовали.");

                _money -= _moneyForFailure;

                if (_money < 0)
                    _money = 0;

                return;
            }

            _car.InstallDetail(detail);
            _storage.Remove(detail);

            _renderer.DrawText($"Вы заменили {detailName}.");

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

    abstract class Menu
    {
        private const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        private const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        private const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        protected string[] _items;
        protected int _itemIndex = 0;

        private Renderer _renderer = new Renderer();
        private bool _isRunning;

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                _renderer.DrawMenu(_items, _itemIndex);

                ReadKey();
            }
        }

        protected virtual void ConfirmActionSelection()
        {
            _renderer.EraseColumnText(_items.Length, true);
            _renderer.EraseText();
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

    class Renderer
    {
        private readonly int detailsForRepairCursorPositionY = 7;
        private int textCursorPositionY = 5;
        private int _storageCursorPositionY = 12;

        private ConsoleColor _backgroundColor = ConsoleColor.White;
        private ConsoleColor _foregroundColor = ConsoleColor.Black;

        private int _spaceLineSize = 100;
        private char _spaceChar = ' ';

        public void DrawStorage(string[] storageInfo)
        {
            for (int i = 0; i < storageInfo.Length; i++)
                DrawText(storageInfo[i], _storageCursorPositionY + i);
        }

        public void DrawRepairInfo(List<string> detailsName)
        {
            Console.SetCursorPosition(0, detailsForRepairCursorPositionY);

            Console.WriteLine("Необходимо отремонтировать:");

            detailsName.ForEach(name => Console.WriteLine(name));
        }

        public void DrawMenu(string[] items, int index)
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < items.Length; i++)
                if (i == index)
                    WriteColoredText(items[i]);
                else
                    Console.WriteLine(items[i]);
        }

        public void EraseColumnText(int value, bool isMenu = false)
        {
            int cursorPositionY = isMenu ? 0 : detailsForRepairCursorPositionY;

            Console.SetCursorPosition(0, cursorPositionY);

            for (int i = 0; i < value; i++)
                EraseText(cursorPositionY + i);
        }

        public void DrawText(string text)
        {
            EraseText(textCursorPositionY);

            Console.Write(text);
        }

        public void DrawText(string text, int cursorPosition)
        {
            EraseText(cursorPosition);

            Console.Write(text);
        }

        public void EraseText(int cursorPositionY)
        {
            Console.SetCursorPosition(0, cursorPositionY);
            Console.Write(new string(_spaceChar, _spaceLineSize));
            Console.CursorLeft = 0;
        }

        public void EraseText()
        {
            Console.SetCursorPosition(0, textCursorPositionY);
            Console.Write(new string(_spaceChar, _spaceLineSize));
            Console.CursorLeft = 0;
        }


        public void WriteColoredText(string text)
        {
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.WriteLine(text);
            Console.ResetColor();
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
}
