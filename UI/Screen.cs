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
        public static ConsoleColor FieldForeground = ConsoleColor.White;
        public static ConsoleColor FieldBackground = ConsoleColor.DarkGray;
        static Stack<Screen> screen_stack = new();
        static Dictionary<ConsoleKey, Action> keyActions = new();
        private bool quit = false;

        public static void Display(Screen screen)
        {
            screen_stack.Push(screen);
            ConsoleKey key;
            do
            {
                Clear();
                keyActions = new();
                screen_stack.Peek().Draw();
                if (screen.quit)
                    break;
                key = Console.ReadKey().Key;
                if (keyActions.ContainsKey(key)) {
                    keyActions[key]();
                }
            } while (true);
        }
        public void ExitOnEscape() 
        {
            AddKey(ConsoleKey.Escape, () => { screen_stack.Pop(); });
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

        static void fastClear()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < Console.WindowHeight; y++)
            {
                Console.WriteLine(string.Format("{0," + (Console.WindowWidth - 1) + "}", ""));
            }
            Console.SetCursorPosition(0, 0);
        }
        public static void Clear()
        {
            ColorDefault();
            fastClear();
            Console.WriteLine(string.Format("==={0}===", Screen.BreadCrumbs));
        }
        public void Quit()
        {
            quit = true;
            screen_stack.Pop();
            Clear();
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
