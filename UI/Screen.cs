using System;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI
{
    public abstract class Screen
    {
        public abstract string Title { get; set; }
        public bool Show { get { return !quit; }}
        public static ConsoleColor DefaultForeground = ConsoleColor.White;
        public static ConsoleColor DefaultBackground = ConsoleColor.Black;
        public static ConsoleColor FocusForeground = ConsoleColor.Black;
        public static ConsoleColor FocusBackground = ConsoleColor.White;
        public static ConsoleColor FieldForeground = ConsoleColor.White;
        public static ConsoleColor FieldBackground = ConsoleColor.DarkGray;
        static Screen current = null;
        private bool quit = false;
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
        public static void ColorDefault() 
        {
            Console.BackgroundColor = DefaultBackground;
            Console.ForegroundColor = DefaultForeground;
        }
        public static void ColorField()
        {
            Console.BackgroundColor = FieldBackground;
            Console.ForegroundColor = FieldForeground;
        }
        public static void ColorFocus()
        {
            Console.BackgroundColor = FocusBackground;
            Console.ForegroundColor = FocusForeground;
        }
        public static void Clear(Screen screen)
        {   
            fastClear();
            Console.WriteLine(string.Format("==={0}===", screen.Title));
        }
        static void fastClear()
        {
            StringBuilder b = new();
            for (int y=0; y < Console.WindowHeight; y++) 
            {
                b.AppendFormat("{0,"+Console.WindowWidth+"}\n", "");
            }
            Console.SetCursorPosition(0,0);
            Console.Write(b);
            Console.SetCursorPosition(0,0);
        }
        public static void Clear() {
            ColorDefault();
            Clear(current);
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
