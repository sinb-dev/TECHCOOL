using System;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI 
{
    public class ScreenExtended
    {
        public Screen screen { get; set; }
        public string Text { get; set; }
    }
    public class Menu
    {
        List<ScreenExtended> screens = new List<ScreenExtended>();
        int index = 0;
        public int Index { get { return index; } set { index = value % screens.Count; } }
        public Screen Screen { get { return screens.Count > 0 ? screens[index].screen : null; } }
        
        public static ConsoleColor DefaultForeground { get; set; } = ConsoleColor.White;
        public static ConsoleColor DefaultBackground { get; set; } = ConsoleColor.Black;
        public static ConsoleColor FocusForeground { get; set; } = ConsoleColor.Black;
        public static ConsoleColor FocusBackground { get; set; } = ConsoleColor.White;
        
        public void Add(Screen screen, string Text = null)
        {
            ScreenExtended thisscreen = new ScreenExtended();
            thisscreen.screen = screen;
            thisscreen.Text = Text;
            screens.Add(thisscreen);
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
                    Console.BackgroundColor = FocusBackground;
                    Console.ForegroundColor = FocusForeground;
                }
                if (screens[i].Text != null)
                {
                    Console.WriteLine($"{i + 1}. {screens[i].Text}");
                }
                else
                {
                    Console.WriteLine($"{i + 1}. {screens[i].screen.Title}");
                }
                Console.BackgroundColor = DefaultBackground;
                Console.ForegroundColor = DefaultForeground;
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
                Screen.Clear(screen);
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