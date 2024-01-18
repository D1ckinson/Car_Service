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
    static class Renderer
    {
        public const int DetailsForRepairCursorPositionY = 7;
        private const int TextCursorPositionY = 5;
        private const int StorageCursorPositionY = 11;

        private const ConsoleColor s_backgroundColor = ConsoleColor.White;
        private const ConsoleColor s_foregroundColor = ConsoleColor.Black;

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

        public static void WriteColoredText(string text, ConsoleColor frontColor = s_foregroundColor, ConsoleColor backColor = s_backgroundColor)
        {
            Console.ForegroundColor = frontColor;
            Console.BackgroundColor = backColor;

            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
