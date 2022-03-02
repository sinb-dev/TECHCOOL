using System;
using System.Collections.Generic;

namespace TECHCOOL 
{
    public class Panel : Component
    {
        bool showBorder = true;
        public string Title = "Untitled panel";
        public string BottomText = "";
        
        private void put(char chr, int x, int y) 
        {
            Console.SetCursorPosition(x,y);
            Console.Write(chr);
        }
        private void put(string str, int x, int y) 
        {
            Console.SetCursorPosition(x,y);
            Console.Write(str);
        }
        public override void Render(int ox, int oy, int width, int height) 
        {
            //Draw border
            if (showBorder) 
            {
                int pad = (width-2);
                string titleText = string.Format(" {0} ",Title);
                put(titleText.PadLeft((pad+titleText.Length)/2,'-').PadRight(pad,'-'),ox+1,oy);

                string bottomText = BottomText.Length > 0 ? string.Format(" {0} ",BottomText) : "";
                put(bottomText.PadLeft((pad+bottomText.Length)/2,'-').PadRight(pad,'-'),ox+1,height);

                for (int y = oy+1; y < height-1; y++) {
                    put('|', oy, y);
                    put('|', width, y);
                }

                //Draw corners
                put('+', ox,oy);
                put('+', width,oy);
                put('+', oy,height);
                put('+', width,height);
            
                ox += 1;
                oy += 1;
            }
            foreach (var child in children) {
                child.Render(ox,oy, width-2, height-2);
            }
        }
    }
}