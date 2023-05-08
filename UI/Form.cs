using System;
using System.Collections.Generic;

namespace TECHCOOL.UI
{
    public class Form<T>
    {
        
        T record;
        Dictionary<string, Field> fields = new();
        int field_edit_index = 0;
        string current_field { get {
            int i = 0;
            string title = "";
            foreach (KeyValuePair<string,Field> kv in fields) 
            {
                if (i == field_edit_index)
                {
                    title = kv.Key;
                    break;
                }
                i++;
            }
            return title;
            }
        }

        public Form<T> TextBox(string title, string property) 
        {
            fields.Add(title, new TextBox { Title = title, Property = property});
            return this;
        }
        public Form<T> IntBox(string title, string property) 
        {
            fields.Add(title, new IntBox { Title = title, Property = property});
            return this;
        }
        public Form<T> DoubleBox(string title, string property) 
        {
            fields.Add(title, new DoubleBox { Title = title, Property = property});
            return this;
        }
        public Form<T> SelectBox(string title, string property, Dictionary<string,object> options=null) 
        {
            if (options == null) options = new();
            fields.Add(title, new SelectBox { Title = title, Property = property, Options = options});
            return this;
        }
        public void AddOption(string field, string option, object value)
        {
            if (!fields.ContainsKey(field)) 
            {
                throw new Exception("There is no such field in this form called "+field);
            }
            if (!(fields[field] is SelectBox))
            {
                throw new Exception("Field "+field+" is not a SelectBox");
            }
            var f = (SelectBox) fields[field];
            f.Options.Add(option,value);
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
            else if (typeof(decimal) == property.PropertyType)
            {
                decimal v = 0;
                decimal.TryParse(value, out v);
                property.SetValue(record,v);
            }

            //property.SetValue(record,)
        }
        public void Edit(T record)  
        {
            this.record = record;
            //Copy values from record into fields

            foreach (KeyValuePair<string,Field> kv in fields) 
            {
                var field = kv.Value;
                var prop = record.GetType().GetProperty(field.Property);
                if (prop == null)
                {
                    throw new($"Form cannot edit: There is no property named '{field.Property}' in class '{record.GetType()}");
                }
                var value = prop.GetValue(record);
                if (value != null) {
                    field.Value = value;
                }
            }

            int x,y;
            (x,y) = Console.GetCursorPosition();
            ConsoleKeyInfo key;
            do
            {
                Console.SetCursorPosition(x,y);
                field_edit_index = Math.Clamp(field_edit_index, 0, fields.Count-1);
                Draw();
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        fields[current_field].Enter();
                        processValue(fields[current_field].Property, fields[current_field].Value.ToString());
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
        protected void Draw() 
        {
            int x,y;
            (x,y) = Console.GetCursorPosition();
            int i = 0;
            foreach (KeyValuePair<string,Field> kv in fields) 
            {
                Field field = kv.Value;
                field.Focus = (i++ == field_edit_index); 
                field.Draw(x,y++);
            }
        }

        int getLongestTitleLength() 
        {
            int longest = 0;
            foreach (KeyValuePair<string,Field> kv in fields) 
            {
                Field field = kv.Value;
                longest = Math.Max(field.Title.Length,longest);
            }
            return longest;
        }
    }
    public abstract class Field {
        public string Title { get; set; }
        public string Property { get; set; }
        public abstract object Value { get; set; }
        public bool Focus {get; set;}
        public int FieldWidth {get;set;} = 20;
        public int LabelWidth {get;set;} = 20;
        public abstract void Draw(int left, int top);
        public abstract void Enter();

    }
    public class TextBox : Field
    {
        string value;
        public override object Value { get {return value;} set { this.value = value.ToString();}}
        int left = 0;
        int top = 0;
        
        public override void Enter() 
        {
            Console.SetCursorPosition(left+LabelWidth,top);
            Console.CursorVisible = true;
            Screen.ColorFocus();
            Value = Console.ReadLine();
            Console.CursorVisible = false;
            Screen.ColorDefault();
        }
        public override void Draw(int left, int top) {
            this.left = left;
            this.top = top;
            Console.SetCursorPosition(left,top);
            Console.Write("{0,-"+LabelWidth+"}", Title);
            if (Focus)
                Screen.ColorFocus();
            else
                Screen.ColorField();

            
            Console.Write($"{{0,-{FieldWidth}}}",Value);
            Screen.ColorDefault();
        }
    }
    public class IntBox : TextBox
    {
        int value;
        public override object Value { get {return value;} set { int.TryParse(value.ToString(), out this.value); }}
        int left = 0;
        int top = 0;
        
        public override void Enter() 
        {
            Console.SetCursorPosition(left+LabelWidth,top);
            Console.CursorVisible = true;
            Screen.ColorFocus();
            string input = Console.ReadLine();
            int.TryParse(input, out value);
            
            Console.CursorVisible = false;
            Screen.ColorDefault();
        }
        public override void Draw(int left, int top) {
            this.left = left;
            this.top = top;
            Console.SetCursorPosition(left,top);
            Console.Write("{0,-"+LabelWidth+"}", Title);
            if (Focus)
                Screen.ColorFocus();
            else
                Screen.ColorField();

            
            Console.Write($"{{0,-{FieldWidth}}}",Value);
            Screen.ColorDefault();
        }
    }
    public class DoubleBox : TextBox
    {
        int value;
        public override object Value { get {return value;} set { int.TryParse(value.ToString(), out this.value); }}
        int left = 0;
        int top = 0;
        
        public override void Enter() 
        {
            Console.SetCursorPosition(left+LabelWidth,top);
            Console.CursorVisible = true;
            Screen.ColorFocus();
            string input = Console.ReadLine();
            int.TryParse(input, out value);
            
            Console.CursorVisible = false;
            Screen.ColorDefault();
        }
        public override void Draw(int left, int top) {
            this.left = left;
            this.top = top;
            Console.SetCursorPosition(left,top);
            Console.Write("{0,-"+LabelWidth+"}", Title);
            if (Focus)
                Screen.ColorFocus();
            else
                Screen.ColorField();

            
            Console.Write($"{{0,-{FieldWidth}}}",Value);
            Screen.ColorDefault();
        }
    }
    public class SelectBox : Field
    {
        object value;
        public override object Value { get {return value;} set { this.value = value; }}
        public Dictionary<string,object> Options = new();
        int left = 0;
        int top = 0;
        int index = 0;
        public override void Enter() 
        {
            

            
            bool stop = false;
            object currentValue = value;
            Func<int,object> valueByIndex = (idx) => {
                int ii = 0;
                foreach(KeyValuePair<string,object> kv in Options) {
                    if (ii++ == idx) return kv.Value;
                }
                return null;
            };
            do {
                int i = 0;
            
                foreach(KeyValuePair<string, object> kv in Options)
                {
                    Console.SetCursorPosition(left+LabelWidth,top+1+i);
                    if (i == index)
                        Screen.ColorFocus();
                    else
                        Screen.ColorDefault();

                    Console.Write("{0,"+FieldWidth+"}", kv.Key);
                    i++;
                }


                ConsoleKey key = Console.ReadKey().Key;
                index = Math.Clamp(index, 0, Options.Count-1);
                switch (key)
                {
                    case ConsoleKey.Enter:
                        value = valueByIndex(index);
                        //Need to clear and redraw this screen
                        Screen.Clear();
                        stop = true;
                        break;
                    case ConsoleKey.UpArrow:
                        index--;
                        break;
                    case ConsoleKey.DownArrow:
                        index++;
                        break;
                    case ConsoleKey.Escape:
                        stop = true;
                        break;
                }
            } while(!stop);
            
            Console.CursorVisible = false;
            Screen.ColorDefault();
        }
        public override void Draw(int left, int top) {
            this.left = left;
            this.top = top;
            Console.SetCursorPosition(left,top);
            Console.Write("{0,-"+LabelWidth+"}", Title);
            if (Focus)
                Screen.ColorFocus();
            else
                Screen.ColorField();

            
            Console.Write($"{{0,-{FieldWidth}}}",Value);
            Screen.ColorDefault();
        }
    }
}

