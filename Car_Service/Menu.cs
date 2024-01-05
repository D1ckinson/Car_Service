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

        protected virtual void ConfirmActionSelection() =>
            Console.Clear();

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
}
