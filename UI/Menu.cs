using System;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI 
{
    public class Menu
    {
        List<Screen> screens = new List<Screen>();
        int index = 0;
        public int Index { get { return index; } set { index = value % screens.Count; } }
        public Screen Screen { get { return screens.Count > 0 ? screens[index] : null; } }
        public void Add(Screen screen)
        {
            screens.Add(screen);
        }
        public void Up()
        {
            index--;
            if (index < 0) index = screens.Count - 1;
        }

        public void Down()
        {
            index++;
            if (index >= screens.Count) index = 0;
        }

        public void Draw()
        {
            ConsoleKeyInfo key;
            for (int i = 0; i < screens.Count; i++)
            {
                if (index == i)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine($"{i+1}. {screens[i].Title}");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}