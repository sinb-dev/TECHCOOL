using System;
using TECHCOOL.UI;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI
{
    public class ListPage<T>
    {
        const char H_BORDER_CHARACTER = '-';
        const char V_BORDER_CHARACTER = '|';
        class Column {
            public string Title { get; set; }
            public string Property { get; set; }
            public int Width { get; set; }
            public Func<object,string> ValueProcessor { get; set;}
        }
        Dictionary<string, Column> columns = new();
        Dictionary<ConsoleKey, Action<T>> keyActions = new();
        List<T> records = new();
        int selected_index = 0;
        bool select = false;

        public void Up()
        {
            selected_index--;
            if (selected_index < 0) selected_index = records.Count - 1;
        }

        public void Down()
        {
            selected_index++;
            if (selected_index >= records.Count) selected_index = 0;
        }

        public ListPage<T> AddColumn(string title, string property, int width = 15, Func<object,string> valueProcessor =null) 
        {
            if (string.IsNullOrWhiteSpace(property)) {
                Console.WriteLine("ListPage: Adding column with title '"+title+"' has an invalid property!");
                return this;
            }
            if (valueProcessor == null)
                valueProcessor = (o) => (o!= null) ? o.ToString() : "";
            columns[property] = new Column { Title = title, Property = property, Width = width, ValueProcessor = valueProcessor };
            return this;
        }
        public ListPage<T> AddColumn(string title, string property, int width, string format) 
        {
            if (string.IsNullOrWhiteSpace(property)) {
                Console.WriteLine("ListPage: Adding column with title '"+title+"' has an invalid property!");
                return this;
            }
            if (format == null) format = ","+width;
            else if (format.Substring(0,1) != ":" && format.Substring(0,1) != ",") format = ","+format;
            Func<object,string> valueProcessor = (o) => string.Format("{0"+format+"}",o);
            columns[property] = new Column { Title = title, Property = property, Width = width, ValueProcessor = valueProcessor };
            return this;
        }

        public ListPage<T> AddKey(ConsoleKey key, Action<T> callback)
        {
            keyActions[key] = callback;
            return this;
        }

        public ListPage<T> Add(T record) 
        { 
            records.Add(record);
            return this;
        }
        public ListPage<T> Add(IEnumerable<T> record) 
        { 
            IEnumerator<T> e = record.GetEnumerator();
            while (e.MoveNext()) {
                records.Add(e.Current);
            }
            return this;
        }
        public bool Remove(T record)
        {
            return records.Remove(record);
        }
        public void Draw()
        {
            StringBuilder sb = new StringBuilder();
            int total_width = getWidth();
            if (total_width < 2) return;
            
            sb.Append(H_BORDER_CHARACTER+"".PadRight(total_width-2,H_BORDER_CHARACTER)+H_BORDER_CHARACTER);
            sb.Append("\n"+V_BORDER_CHARACTER);
            foreach (KeyValuePair<string, Column> kv in columns) 
            {
                int width = kv.Value.Width;
                sb.AppendFormat("{0, "+width+"}{1}", kv.Value.Title, V_BORDER_CHARACTER);
            }
            sb.Append("\n");
            sb.Append("".PadRight(getWidth(),H_BORDER_CHARACTER));
            Console.WriteLine(sb);
            sb.Clear();
            var i = 0;
            foreach (T r in records) {
                if (select && selected_index == i++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                
                sb.Append(V_BORDER_CHARACTER);
                foreach (KeyValuePair<string, Column> kv in columns) 
                {
                    try {
                        var prop = r.GetType().GetProperty(kv.Value.Property);
                        var val = kv.Value.ValueProcessor(prop.GetValue(r));

                        int width = kv.Value.Width;
                        sb.AppendFormat("{0, "+width+"}{1}", val,V_BORDER_CHARACTER);
                    } catch(NullReferenceException e) {
                        //Nooos
                        Console.WriteLine($"There is no property on class '{r.GetType()}' called '{kv.Value.Property}'\n"+e);
                    }
                }
                Console.WriteLine(sb);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                sb.Clear();
            }
            sb.Append("".PadRight(getWidth(),H_BORDER_CHARACTER));
            Console.WriteLine(sb.ToString());
            int header_height = 3;
            Console.SetCursorPosition(0, selected_index + header_height);
        }
        public int getWidth() {
            int width = columns.Count + 1; //Include borders in with.
            foreach (KeyValuePair<string, Column> kv in columns) 
            {
                width += kv.Value.Width;
            }
            return width;
        }

        public T Select() {

            select = true;
            ConsoleKey key;
            int x,y;
            (x,y) = Console.GetCursorPosition();
            do
            {
                Console.SetCursorPosition(x,y);
                Draw();
                key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.Escape:
                        return default(T);
                    case ConsoleKey.Enter:
                        return records[selected_index];
                    case ConsoleKey.DownArrow:
                        Down();
                        break;
                    case ConsoleKey.UpArrow:
                        Up();
                        break;
                    default:
                        if (keyActions.ContainsKey(key) && records.Contains(records[selected_index]))
                            keyActions[key](records[selected_index]);
                        break;
                        
                }
                
            }
            while (true);
        }
    }
}

