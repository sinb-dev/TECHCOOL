using System;
using System.Collections.Generic;
using System.Text;

namespace TECHCOOL.UI
{
    public class ListPage<T> where T : new()
    {
        const char H_BORDER_CHARACTER = '─';
        const char V_BORDER_CHARACTER = '│';
        const char NW_CORNER = '┌';
        const char NE_CORNER = '┐';
        const char SW_CORNER = '└';
        const char SE_CORNER = '┘';
        const char WEST_T = '├';
        const char EAST_T = '┤';
        const char NORTH_T = '┬';
        const char SOUTH_T = '┴';
        const char CROSS = '┼';

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
        public void Clear()
        {
            records.Clear();
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

            // build horizontalt line graphics
            string UH_LINE = "" + NW_CORNER;
            string MH_LINE = "" + WEST_T;
            string LH_LINE = "" + SW_CORNER;

            int count = 0;
            foreach (KeyValuePair<string, Column> kv in columns)
            {
                int width = kv.Value.Width;
                var fill = "".PadRight(width, H_BORDER_CHARACTER);
                UH_LINE += fill + (count < columns.Count-1 ? NORTH_T : null);
                MH_LINE += fill + (count < columns.Count - 1 ? CROSS : null);
                LH_LINE += fill + (count < columns.Count - 1 ? SOUTH_T : null);
                count++;
            }

            UH_LINE += "" + NE_CORNER;
            MH_LINE += "" + EAST_T;
            LH_LINE += "" + SE_CORNER;

            // draw header
            sb.Append(UH_LINE + Environment.NewLine);

            sb.Append(V_BORDER_CHARACTER);
            foreach (KeyValuePair<string, Column> kv in columns)
            {
                int width = kv.Value.Width;
                sb.AppendFormat("{0, -" + width + "}{1}", kv.Value.Title, V_BORDER_CHARACTER);
            }

            sb.Append(Environment.NewLine);
            sb.Append(MH_LINE);
            Console.WriteLine(sb);

            // draw contents

            sb.Clear();
            var i = 0;
            foreach (T r in records)
            {
                if (select && selected_index == i++)
                {
                    Console.BackgroundColor = Screen.FocusBackground;
                    Console.ForegroundColor = Screen.FocusForeground;
                }

                sb.Append(V_BORDER_CHARACTER);
                foreach (KeyValuePair<string, Column> kv in columns)
                {
                    try
                    {
                        var prop = r.GetType().GetProperty(kv.Value.Property);
                        var val = kv.Value.ValueProcessor(prop.GetValue(r));

                        int width = kv.Value.Width;
                        var output = String.Format("{0, -" + width + "}", val)[0..width];
                        sb.AppendFormat(output + V_BORDER_CHARACTER);
                    }
                    catch (NullReferenceException e)
                    {
                        //Nooos
                        Console.WriteLine($"There is no property on class '{r.GetType()}' called '{kv.Value.Property}'\n" + e);
                    }
                }
                Console.WriteLine(sb);
                Console.BackgroundColor = Screen.DefaultBackground;
                Console.ForegroundColor = Screen.DefaultForeground;
                sb.Clear();
            }
            sb.Append(LH_LINE);
            Console.WriteLine(sb.ToString());
            Console.SetCursorPosition(0, selected_index);
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
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        return records[selected_index];
                    case ConsoleKey.DownArrow:
                        Down();
                        break;
                    case ConsoleKey.UpArrow:
                        Up();
                        break;
                    default:
                        if (keyActions.ContainsKey(key)) {
                            if (records.Count > 0 && records.Contains(records[selected_index])) 
                            {
                                keyActions[key](records[selected_index]);
                                return default(T);
                            }    
                            else
                            {
                                keyActions[key](new T());
                            }
                        }
                        else
                        {
                            Console.Beep();
                        }
                        break;
                }    
            }
            while (true);
        }
    }
}
