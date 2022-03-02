using System;
using System.Collections.Generic;

namespace TECHCOOL
{
    public class UILet 
    {
        public static Field Focus {get;set;}    
        public static void Update() 
        {
            if (Focus != null) 
            {
                Console.SetCursorPosition(Focus.RenderInfo.CursorX, Focus.RenderInfo.CursorY);
            }
        }
    }
}