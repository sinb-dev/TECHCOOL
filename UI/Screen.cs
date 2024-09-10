using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TECHCOOL.UI
{
    public abstract class Screen
    {
        public abstract string Title { get; set; }
        public bool Show { get { return !quit; } }
        public static ConsoleColor DefaultForeground = ConsoleColor.White;
        public static ConsoleColor DefaultBackground = ConsoleColor.Black;
        public static ConsoleColor FocusForeground = ConsoleColor.Black;
        public static ConsoleColor FocusBackground = ConsoleColor.White;
        public static ConsoleColor EditForeground = ConsoleColor.Black;
        public static ConsoleColor EditBackground = ConsoleColor.White;
        public static ConsoleColor FieldForeground = ConsoleColor.White;
        public static ConsoleColor FieldBackground = ConsoleColor.DarkGray;
        public static ConsoleColor ErrorForeground = ConsoleColor.Red;
        public static ConsoleColor ErrorBackground = ConsoleColor.Gray;
        static Stack<Screen> screen_stack = new();
        private Dictionary<ConsoleKey, Action> keyActions = new();
        private bool quit = false;

        public static void Display(Screen screen)
        {
            screen_stack.Push(screen);
            ConsoleKey key;
            do
            {
                Clear();
                screen_stack.Peek().Draw();
                if (screen.keyActions.Count > 0)
                {
                    key = Console.ReadKey(true).Key;
                    if (screen.keyActions.ContainsKey(key))
                    {
                        screen.keyActions[key]();
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
            } while (!screen.quit);

        }
        public void ExitOnEscape() 
        {
            AddKey(ConsoleKey.Escape, () => Quit() );
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
        public static void ColorError()
        {
            Console.BackgroundColor = ErrorBackground;
            Console.ForegroundColor = ErrorForeground;
        }
        public static void ColorEdit()
        {
            Console.BackgroundColor = EditBackground;
            Console.ForegroundColor = EditForeground;
        }
        /*
        static void fastClear()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < Console.WindowHeight; y++)
            {
                Console.WriteLine(string.Format("{0," + (Console.WindowWidth - 1) + "}", ""));
            }
            Console.SetCursorPosition(0, 0);
        }
        */
        public static void Clear()
        {
            ColorDefault();
            Console.Clear();
            Console.WriteLine(string.Format("==={0}===", Screen.BreadCrumbs));
        }
        public void Quit()
        {
            quit = true;
            screen_stack.Pop();
        }

        public void AddKey(ConsoleKey key, Action callback)
        {
            keyActions[key] = callback;
        }

        public static string BreadCrumbs {
            get {
                List<string> path = new();
                foreach (Screen screen in screen_stack) 
                {
                    path.Add(screen.Title);
                }
                path.Reverse();
                return String.Join(" > ", path);
            }
        }
        protected abstract void Draw();
    }
    public class EscapeScreen : Screen
    {
        public override string Title { get; set; } = "";
        protected override void Draw() { }
    }
}
