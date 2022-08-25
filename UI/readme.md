## Opret ny skærm
En skærm er en klasse, som tegner en titel og noget indhold i konsollen. Det gør den ved hjælp af en Draw() metode, hvor det står brugeren frit for at bruge dotnets indbyggede Console metoder til at tegne på skærmen (WriteLine, SetCursorPosition osv). På den måde giver en skærm store friheder.
Hvis brugeren trykker på en tast vil skærmklassen tegne skærmen igen.

1. Opret en tom klasse der arver fra TECHCOOL.UI.Screeen

```C#
using TECHCOOL.UI;
public class MyFirstScreen : Screen
{
	public override string Title { get; set; } = "My first screen"; 
	protected override void Draw()
	{
		Clear(this);
		Console.WriteLine("My first screen!");
	}
}
```

2. Vi bruger skærmen ved at bruge den statiske metode Display() på Screen klassen.
```C#
public class Program 
{
	public static void Main(string[] args)
	{
		MyFirstScreen firstScreen = new MyFirstScreen();
		Screen.Display(firstScreen);
	}
}
```
3. Det munder ud i følgende output i konsollen
```
===My first screen===
My first screen! 
```

## Opret en liste
I dette eksempel vises oprettelse af en liste i TECHCOOL. Det første vi skal bruge er en dataklasse. Derefter instantierer vi en liste og fortæller den hvilke properties på dataklassen, som vi er interesserede i at vise i listen.

1. Opret dataklassen. Denne dataklasse indeholder en titel, en prioritet og en tilstand, som afgør om en todo er i gang
```C#
public class Todo
{
	public enum TodoState { Todo, Started, Done }
	public string Title {get;set;} = "";
	public int Priority {get;set;}
	public TodoState State {get;set;}
	public Todo(string title, int priority=1) 
	{
		Title = title;
		Priority = priority;
	}
}
```
2. Opret en ny skærm: TodoListPage.cs
 ```C#
public class TodoListScreen : Screen
{
	public override string Title { get; set; } = "List of tasks to do"; 
	protected override void Draw()
	{
	  Clear(this); //Clean the screen
		//Gonna draw a list page here
	}
}
```
3. I Draw metoden laver vi en instans af en ListPage. Vores ListPage skal vide hvilken type klasse, som den skal vise. Det fortæller vi den ved at bruge en generic
`ListPage<Todo> listPage = new ListPage<Todo>();`

4. Tilføj noget data til listen
```C#
//...
listPage.Add(new Todo("Buy milk"));
listPage.Add(new Todo("Walk the dog", 2));
listPage.Add(new Todo("Clean toilet"));
```

5. Fortæl vores ListPage hvilke kolonner den skal vise. 
```C#
//...
listPage.AddColumn("Todo", "Title");
```
Her konfigureres listen til at vise én kolonne. Øverst i listen vil der stå "Todo" som angivet i det første parameter i AddColumn() og under overskriften vil propertyen _Title_ fra Todo klassen blive vist.

6. Tegn listen
Listen kan blive tegnet på skærmen gennem metoden Draw(). 
```C#
Clear(this); //Clean the screen
ListPage<Todo> listPage = new ListPage<Todo>();
listPage.Add(new Todo("Buy milk"));
listPage.Add(new Todo("Walk the dog", 2));
listPage.Add(new Todo("Clean toilet"));

listPage.AddColumn("Todo", "Title"); //Add column to the list

listPage.Draw(); //Draw the screen
```
7. Brug den nye TodoListScreen sammen med Screen.Display()
`Screen.Display( new TodoListScreen() );`
Output
```
===List of tasks to do===
-----------------
|           Todo|
-----------------
|       Buy milk|
|   Walk the dog|
|   Clean toilet|
-----------------
```
8. Prøv selv at tilføje kolonnerne for propertierne **Priority** og **State** i Todo klassen
9. Vælg en Todo fra listen ved at bruge metoden **Select()** på listPage i stedet for **Draw()**
```C#
//...
Todo selected = listPage.Select(); //Select todo from the list
Console.WriteLine("You selected: "+selected.Title);
```
Dette gør listen interaktiv og lader brugeren vælge en fra listen.

10. Færdigt eksempel
```C#
    public class TodoListScreen : Screen
    {
        public override string Title { get; set; } = "List of tasks to do"; 
        protected override void Draw()
        {
	    do
	    {
                 Clear(this); //Clean the screen
		 ListPage<Todo> listPage = new ListPage<Todo>();
                 listPage.Add(new Todo("Buy milk"));
                 listPage.Add(new Todo("Walk the dog", 2));
                 listPage.Add(new Todo("Clean toilet")); 

                 listPage.AddColumn("Todo", "Title");
		
                 Todo selected = listPage.Select();
		 if (selected != null)
		 {
		     Screen.Display(new EditTodoScreen(selected))
                 }
		 else
                 {
		      Quit();
		      return;
		 }
             } while (Show);
        }
    }
```

## Redigering af klasse
Denne guide viser hvordan du kan redigere en klasse ved hjælp af TECHCOOL konsol UI. Guiden tager udgangspunkt i samme klasse som guiden "Opret en liste", nemlig Todo klassen
```C#
public class Todo
{
	public enum TodoState { Todo, Started, Done }
	public string Title {get;set;} = "";
	public int Priority {get;set;}
	public TodoState State {get;set;}
	public Todo(string title, int priority=1) 
	{
		Title = title;
		Priority = priority;
	}
}
```

1. Opret en ny skærm: EditTodoScreen

```C#
public class EditTodoScreen : Screen
{
	public override string Title { get; set; } = "Edit todo"; 
	protected override void Draw()
	{
		Clear(this); //Clean the screen
		Todo todo = new Todo("New todo");
		
		//Gonna edit a todo
		
	}
}
```
2. Opret en instans af TECHCOOL.UI.Form og put en TextBox i den.

```C#
//...
//Create instance
Form<Todo> editor = new Form<Todo>();

//Add a textbox
editor.TextBox("Todo name", "Title");

//Draw the editor
editor.Edit(todo);

```
Der er forskellige kontroller, som kan sættes op i en editor. Kontrollerne er afhængige af propertiens datatype:
* TextBox til strings
* SelectBox til flere valg (f.eks. enums)
* IntBox til int
* DoubleBox til double
Som default kan TextBox bruges 
3. Færdigt eksempel
```C#
public class EditTodoScreen : Screen
    {
        public override string Title { get; set; } = "Edit todo"; 
        protected override void Draw()
        {
            Clear(this); //Clean the screen
            Todo todo = new Todo("New todo");

            Form<Todo> editor = new Form<Todo>();
            editor.TextBox("Todo", "Title");
            editor.IntBox("Importance", "Priority");
            editor.SelectBox("Progress", "State");
            editor.AddOption("Progress", "Not started", Todo.TodoState.Todo);
            editor.AddOption("Progress", "Begun", Todo.TodoState.Started);
            editor.AddOption("Progress", "Complete", Todo.TodoState.Done);
            editor.Edit(todo);

            Clear(this);
            Console.WriteLine($"Todo {todo.Title} is {todo.State}");
        }
    }
```

## Lav en menu
Menuer er en samling af skærme (Screens), som den lader brugeren vælge imellem. Vi tager udgangspunkt i de tre skærme vi har lavet i denne guide.
* MyFirstScreen
* TodoListScreen
* EditTodoScreen

1. Opret en ny skærm: MyMenuScreen
```C#
public class MyMenuScreen : Screen
{
	public override string Title { get; set; } = "Menu Example"; 
	protected override void Draw()
	{
		//Gonna draw a menu here!
	}
}
```

2. Opret en Menu instans og tilføj skærme til den
```C#
//...
Menu menu = new Menu();
//Add our screens
menu.Add(new MyFirstScreen());
menu.Add(new TodoListScreen());
menu.Add(new EditTodoScreen());
menu.Start(this); //Start the menu letting the user pick a menu item:
```

3. Brug nu menuen i din main metode:
```C#
public static void Main(string[] args)
{
	Screen.Display(new MyMenuScreen());
}
```
