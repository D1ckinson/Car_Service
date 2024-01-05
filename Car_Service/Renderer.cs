using System;

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
    static class Renderer
    {
        private static ConsoleColor s_backgroundColor = ConsoleColor.White;
        private static ConsoleColor s_foregroundColor = ConsoleColor.Black;

        public const int ResponseCursorPositionY = 7;
        public const int RequestCursorPositionY = 5;
        public const int BooksCursorPositionY = 9;

        private const int SpaceLineSize = 100;
        private const char SpaceChar = ' ';

        public static void DrawMenu(string[] items, int index)
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < items.Length; i++)
                if (i == index)
                    WriteColoredText(items[i]);
                else
                    Console.WriteLine(items[i]);
        }

        public static void EraseColumnText(int counter, int cursorPositionY)
        {
            for (int i = 0; i < counter; i++)
                EraseText(i + cursorPositionY);
        }

        public static void DrawText(string text, int cursorPositionY)
        {
            EraseText(cursorPositionY);

            Console.Write(text);
        }

        public static void EraseText(int cursorPositionY)
        {
            Console.SetCursorPosition(0, cursorPositionY);

            Console.Write(new string(SpaceChar, SpaceLineSize));

            Console.CursorLeft = 0;
        }

        private static void WriteColoredText(string text)
        {
            Console.ForegroundColor = s_foregroundColor;
            Console.BackgroundColor = s_backgroundColor;

            Console.WriteLine(text);

            Console.ResetColor();
        }
    }
}
