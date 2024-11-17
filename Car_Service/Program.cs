using System;
using System.Collections.Generic;
using System.Linq;

namespace Car_Service
{
    internal class Program
    {
        static void Main()
        {
            CarServiceFactory carServiceFactory = new CarServiceFactory();
            CarService carService = carServiceFactory.Create();

            carService.Work();
        }
    }

    class Detail
    {
        public Detail(string name, bool isWorking)
        {
            Name = name;
            IsWorking = isWorking;
        }

        public bool IsWorking { get; }
        public string Name { get; }
    }

    class Car
    {
        private List<Detail> _details;

        public Car(List<Detail> details)
        {
            _details = details;
        }

        public List<string> BrokenDetailsNames => _details.Where(detail => detail.IsWorking == false).Select(detail => detail.Name).ToList();
        //
        public void ReplaceDetail(Detail detail)
        {
            Detail detailToRemove = _details.Find(matchedDetail => matchedDetail.Name == detail.Name);

            _details.Remove(detailToRemove);
            _details.Add(detail);
        }
    }

    class CarFactory
    {
        private DetailsFactory _detailFabrik = new DetailsFactory();

        public Queue<Car> Create(int quantity)
        {
            Queue<Car> cars = new Queue<Car>();

            for (int i = 0; i < quantity; i++)
            {
                List<string> detailsNames = _detailFabrik.Names;
                List<Detail> details = _detailFabrik.Create(detailsNames);

                cars.Enqueue(new Car(details));
            }

            return cars;
        }
    }

    class CarServiceFactory
    {
        private CarFactory _carFactory = new CarFactory();
        private DetailsFactory _detailFactory = new DetailsFactory();

        public CarService Create()
        {
            int carsQuantity = 5;

            Queue<Car> cars = _carFactory.Create(carsQuantity);
            List<Detail> details = FillDetails();
            List<string> detailsNames = _detailFactory.Names;

            return new CarService(cars, details, detailsNames);
        }

        private List<Detail> FillDetails()
        {
            List<string> detailsNames = _detailFactory.Names;
            List<Detail> details = new List<Detail>();
            int detailsQuantity = 6;

            foreach (string name in detailsNames)
            {
                for (int i = 0; i < detailsQuantity; i++)
                {
                    details.Add(_detailFactory.Create(name));
                }
            }

            return details;
        }
    }

    class CarService
    {
        private Queue<Car> _cars;
        private List<Detail> _details;
        private List<string> _detailsNames;
        private int _money;

        public CarService(Queue<Car> cars, List<Detail> details, List<string> detailsNames)
        {
            _cars = cars;
            _details = details;
            _detailsNames = detailsNames;
            _money = 10000;
        }

        public void Work()
        {


            while (_cars.Count > 0)
            {
                Car car = _cars.Dequeue();

                RepairCar(car);
            }
        }

        private void RepairCar(Car car)
        {
            bool isAnyReplaced = false;
            int penaltyCost = -500;
            int repairCost = 400;


            while (car.BrokenDetailsNames.Count > 0)
            {

                Console.WriteLine($"Машин в очереди на починку: {_cars.Count}\n");
                Console.WriteLine("Сломанные детали в этой машине:");
                car.BrokenDetailsNames.ForEach(name => Console.WriteLine(name));
                Console.WriteLine("\n");

                for (int i = 0; i < _detailsNames.Count; i++)
                {
                    int detailNumber = i + 1;
                    Console.WriteLine($"{detailNumber} -  заменить {_detailsNames[i]}");
                }

                int rejectNumber = _detailsNames.Count + 1;
                Console.WriteLine($"{rejectNumber} - отказ от ремонта");

                int input = UserUtils.ReadInt("Введите команду: ");
                int index = input - 1;

                if (input == rejectNumber)
                {
                    if (isAnyReplaced)
                    {
                        penaltyCost *= car.BrokenDetailsNames.Count;
                        Console.WriteLine($"Вы отказались от ремонта начав его и получили {penaltyCost}");
                    }
                    else
                    {
                        Console.WriteLine($"Вы отказались от ремонта и получили {penaltyCost}");
                    }

                    _money += penaltyCost;

                    return;
                }

                if (UserUtils.IsIndexInRange(index, _detailsNames.Count))
                {
                    string detailName = _detailsNames[index];
                    Detail detail = _details.FirstOrDefault(matchedDetail => matchedDetail.Name == detailName);

                    if (detail == null)
                    {
                        Console.WriteLine($"У вас нет {detailName}");

                        continue;
                    }

                    isAnyReplaced = true;

                    if (car.BrokenDetailsNames.Contains(detailName) == false)
                    {
                        _money += penaltyCost;
                        Console.WriteLine($"Вы отремонтировали не ту деталь и получили {penaltyCost}");

                        continue;
                    }

                    _details.Remove(detail);
                    car.ReplaceDetail(detail);

                    _money += repairCost;
                    Console.WriteLine($"Вы отремонтировали деталь и получили {repairCost}");
                }

                Console.WriteLine("Вы отремонтировали машину!");
            }
        }
    }

    class DetailsFactory
    {
        private List<string> _names;

        public DetailsFactory()
        {
            _names = FillNames();
        }

        public List<string> Names => _names.ToList();

        public Detail Create(string name) =>
            new Detail(name, true);

        public List<Detail> Create(List<string> names)
        {
            List<Detail> details = new List<Detail>();
            Queue<bool> detailsStatus = FillDetailsStatus(names.Count);

            for (int i = 0; i < names.Count; i++)
            {
                bool isWork = detailsStatus.Dequeue();
                Detail detail = new Detail(names[i], isWork);
                details.Add(detail);
            }

            return details;
        }

        private Queue<bool> FillDetailsStatus(int detailsQuantity)
        {
            Queue<bool> status = new Queue<bool>();
            int brokenDetailsQuantity = UserUtils.Next(1, detailsQuantity);

            for (int i = 0; i < brokenDetailsQuantity; i++)
            {
                status.Enqueue(false);
            }

            for (int i = status.Count; i < detailsQuantity; i++)
            {
                status.Enqueue(true);
            }

            return status;
        }

        private List<string> FillNames() =>
            new List<string>()
            {
                "Двигатель",
                "Кузов",
                "Подвеска",
                "Рулевая система",
                "Тормозная система",
                "Система питания",
                "Выпускная система",
                "Трансмиссия",
                "Электрооборудование"
            };
    }

    static class UserUtils
    {
        private static Random s_random = new Random();

        public static int ReadInt(string text)
        {
            int number;

            while (int.TryParse(ReadString(text), out number) == false)
                Console.WriteLine("Некорректный ввод. Введите число.");

            return number;
        }

        public static string ReadString(string text)
        {
            Console.Write(text);

            return Console.ReadLine();
        }

        public static int Next(int minValue, int maxValue) =>
            s_random.Next(minValue, maxValue);

        public static bool IsIndexInRange(int index, int maxIndex) =>
            (index < 0 || index >= maxIndex) == false;
    }
}
