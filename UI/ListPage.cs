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
        }
        Dictionary<string, Column> columns = new();
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

        public ListPage<T> AddColumn(string title, string property, int width = 15) 
        {
            if (string.IsNullOrWhiteSpace(property)) {
                Console.WriteLine("ListPage: Adding column with title '"+title+"' has an invalid property!");
                return this;
            }
            columns[property] = new Column { Title = title, Property = property, Width = width };
            return this;
        }

        public void Add(T record) 
        { 
            records.Add(record);
        }
        public void Add(IEnumerable<T> record) 
        { 
            IEnumerator<T> e = record.GetEnumerator();
            while (e.MoveNext()) {
                records.Add(e.Current);
            }
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
                        var val = prop.GetValue(r);
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
            ConsoleKeyInfo key;
            do
            {
                Draw();
                key = Console.ReadKey();
                switch (key.Key)
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
                }
            }
            while (true);
        }
    }
}

