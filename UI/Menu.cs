using System;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI 
{
    public class Menu : Screen
    {
        public override string Title { get; set; } = "Menu";

        List<Screen> screens = new List<Screen>();
        int index = 0;
        public int Index { get { return index; } set { index = value % screens.Count; } }
        public Screen SelectedScreen { get { return screens.Count > 0 ? screens[index] : null; } }

        public Menu()
        {
            AddKey(ConsoleKey.Escape, () =>
                { 
                    ConfirmScreen conf = new("This will exit the menu");
                    Screen.Display(conf);
                    if (conf.confirmed) Quit();
                });
            AddKey(ConsoleKey.DownArrow, () => Down());
            AddKey(ConsoleKey.UpArrow, () => Up());
            AddKey(ConsoleKey.Enter, () => Screen.Display(SelectedScreen));
        }
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

        protected override void Draw()
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
    }
}