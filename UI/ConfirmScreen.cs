using System;

namespace TECHCOOL.UI
{
    public class ConfirmScreen : Screen
    {
        public override string Title { get; set; } = "Confirm";

        public bool confirmed = false;

        public ConfirmScreen(string title = "")
        {
            Title = title;
            AddKey(ConsoleKey.Y, () => confirmed = true);
        }

        protected override void Draw()
        {
            Console.WriteLine("Are you sure (y/n)?");
            Quit();
        }
    }
}
