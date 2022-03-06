using System;
using TECHCOOL.UI;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI
{
    public class Form<T>
    {
        class Field {
            public string Title { get; set; }
            public string Property { get; set; }
        }
        T record;
        List<Field> fields = new();
        int field_edit_index = 0;
        public Form<T> TextBox(string title, string property) 
        {
            fields.Add(new Field { Title = title, Property = property});
            return this;
        }
        protected void Draw() 
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
            Screen.Clear();
            this.record = record;
            Draw();
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
}

