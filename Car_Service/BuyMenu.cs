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

using System.Collections.Generic;
using System;
using System.Linq;

namespace Car_Service
{
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
}
