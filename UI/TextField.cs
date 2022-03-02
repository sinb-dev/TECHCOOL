using System;
using System.Collections.Generic;

namespace TECHCOOL
{
    public class TextField : Field
    {
        public string Value { get;set;} = "";
        public string Name {get;set;} = "";
        public int InputSize {get; set; } = 15;

        public override void Render(int x, int y, int width, int height) 
        {
            var color = Console.BackgroundColor;
            Console.SetCursorPosition(x,y);
            
            Console.Write(Name+" ");
            Console.BackgroundColor = ConsoleColor.Gray;
            int cx = Console.CursorLeft;
            int cy = Console.CursorTop;
            Console.Write(Value.PadLeft(InputSize));
            
            Console.BackgroundColor = color;
            RenderInfo = new RenderInfo(x,y,width,height, cx, cy);
        }
    }
}