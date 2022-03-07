using System;
using System.Collections.Generic;

namespace TECHCOOL.UI
{
    public class Form<T>
    {
        
        T record;
        List<Field> fields = new();
        int field_edit_index = 0;
        Screen screen;
        public Form(Screen screen)
        {
            this.screen = screen;
        }
        public Form<T> TextBox(string title, string property) 
        {
            fields.Add(new TextBox { Title = title, Property = property});
            return this;
        }
        protected void Draw() 
        {
            int x,y;
            (x,y) = Console.GetCursorPosition();
            int i = 0;
            foreach (var field in fields) 
            {
                field.Focus = (i++ == field_edit_index); 
                field.Draw(x,y++);
            }
        }
        protected void Draw_() 
        {
            int x,y;
            (x,y) = Console.GetCursorPosition();
            int titleLength = getLongestTitleLength();
            
            int yi = y;
            foreach (var field in fields) 
            {
                Console.SetCursorPosition(x,yi++);
                Console.Write(field.Title);
            }

            //Position cursor on to current field
            x = titleLength + 1;
            yi = y;
            
            foreach (var field in fields) 
            {
                Console.CursorVisible = true;
                Console.SetCursorPosition(x,yi++);
                string value = Console.ReadLine();
                processValue(field.Property, value);
            }
            
            Console.CursorVisible = false;
            //Field current_field = fields[field_edit_index];
            //Console.SetCursorPosition(x,y+1);
            
        }
        void processValue(string property_name, string value)
        {
            var property = record.GetType().GetProperty(property_name);
            if (property == null ) 
            {
                Console.WriteLine($"Form: No such property '{property_name}' on {record.GetType()}");
                return;
            }

            if (typeof(string) == property.PropertyType) 
            {
                property.SetValue(record,value);
            } 
            else if (typeof(int) == property.PropertyType)
            {
                int v = 0;
                int.TryParse(value, out v);
                property.SetValue(record,v);
            }
            else if (typeof(float) == property.PropertyType)
            {
                float v = 0;
                float.TryParse(value, out v);
                property.SetValue(record,v);
            }
            else if (typeof(double) == property.PropertyType)
            {
                double v = 0;
                double.TryParse(value, out v);
                property.SetValue(record,v);
            }

            //property.SetValue(record,)
        }
        public void Edit(ref T record)  
        {
            this.record = record;

            ConsoleKeyInfo key;
            do
            {
                Screen.Clear(screen);
                Draw();
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Screen.Display(screen);
                        break;
                    case ConsoleKey.DownArrow:
                        field_edit_index++;
                        break;
                    case ConsoleKey.UpArrow:
                        field_edit_index--;
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
                
            }
            while (true);
        }

        int getLongestTitleLength() 
        {
            int longest = 0;
            foreach (var field in fields) 
            {
                longest = Math.Max(field.Title.Length,longest);
            }
            return longest;
        }
    }
    public abstract class Field {
        public string Title { get; set; }
        public string Property { get; set; }
        public bool Focus {get; set;}
        public abstract void Draw(int left, int top);
    }
    public class TextBox : Field
    {
        string value;
        public string Value { get {return value;} set { this.value = value;}}
        public int Width {get;set;} = 20;
        public void Enter() {
        }
        public override void Draw(int left, int top) {
            Console.SetCursorPosition(left,top);
            Console.Write("{0,20}", Title);
            if (Focus)
                Console.BackgroundColor = ConsoleColor.White;
            else
                Console.BackgroundColor = ConsoleColor.Gray;

            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{{0,{Width}}}",Value);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    
}

