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
            for (int i = 0; i < screens.Count; i++)
            {
                if (index == i)
                {
                    Console.BackgroundColor = Screen.FocusBackground;
                    Console.ForegroundColor = Screen.FocusForeground;
                }
                Console.WriteLine($"{i+1}. {screens[i].Title}");
                Console.BackgroundColor = Screen.DefaultBackground;
                Console.ForegroundColor = Screen.DefaultForeground;
            }
        }

        public void Start(Screen screen) 
        {
            ConsoleKeyInfo key;
            int x,y;
            (x,y) = Console.GetCursorPosition();
            
            do
            {
                Console.SetCursorPosition(x,y);
                Screen.Clear();
                Draw();
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Screen.Display(Screen);
                        break;
                    case ConsoleKey.DownArrow:
                        Down();
                        break;
                    case ConsoleKey.UpArrow:
                        Up();
                        break;
                    case ConsoleKey.Escape:
                        screen.Quit();
                        return;
                }

            }
            while (true);
        }
    }
}