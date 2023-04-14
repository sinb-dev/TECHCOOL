using System.Runtime.InteropServices;
using TECHCOOL.UI;

namespace TechCoolFix
{
    internal class Program
    {

        static void Main(string[] args)
        {
            UITest uITest = new UITest();
            Screen.Display(uITest);
        }
    }
}