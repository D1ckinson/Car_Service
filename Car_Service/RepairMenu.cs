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
}
