using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
    internal class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            DetailNames detailNames = new DetailNames();
            CarService carService = new CarService(detailNames);
            ActionBuilder actionBuilder = new ActionBuilder(carService);
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
}
