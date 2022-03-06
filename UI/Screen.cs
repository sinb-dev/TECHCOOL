using System;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI
{
    public abstract class Screen
    {
        public abstract string Title { get; set; }
        static Screen current = null;
        protected bool quit = false;
        public static void Display(Screen screen)
        {
            Clear(screen);
            current = screen;
            ConsoleKey key;
            do
            {
                screen.Draw();
                if (screen.quit) 
                    break;
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.Escape) 
                    break;
            } while (true);
        }
        public static void Clear(Screen screen)
        {   
            Clear();
            Console.WriteLine(string.Format("==={0}===", screen.Title));
        }
        public static void Clear() {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Quit() 
        {
            quit = true;
        }

        protected abstract void Draw();
    }
    public class EscapeScreen : Screen 
    {
        public override string Title { get; set; } = "";
        protected override void Draw() {}
    }
}
