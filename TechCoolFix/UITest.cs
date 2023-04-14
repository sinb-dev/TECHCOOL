using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECHCOOL.UI;

namespace TechCoolFix
{
    public class UITest : Screen
    {
        public override string Title { get; set; } = "Products";

        protected override void Draw()
        {
            var cursorpos = Console.GetCursorPosition();
            Clear(this);
            Console.SetCursorPosition(0, 0);
            int consoleLength = Console.WindowHeight;
            ListPage<UITest> list = new ListPage<UITest>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(new UITest() { Title = $"Product {i}" });
            }

            list.AddColumn("Title", "Title");
            list.Select();


            Console.SetCursorPosition(0, Console.CursorTop); // set cursor position to the end of the last line

        }
    }
}
