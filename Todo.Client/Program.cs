
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Todo.Client;

HttpClient client = new();
client.BaseAddress = new Uri("https://localhost:7216");
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

HttpResponseMessage responseMessage = await client.GetAsync("api/todos/all");
responseMessage.EnsureSuccessStatusCode();

if(responseMessage.IsSuccessStatusCode)
{
    bool keepRunning = true;
    int choice = 0;

    while(keepRunning)
    {
        Console.WriteLine();
        Console.WriteLine("-----------Todo interface-----------");
        Console.WriteLine("1. View all Todos");
        Console.WriteLine("2. Get Todo details by Id");
        Console.WriteLine("3. Create a Todo");
        Console.WriteLine("4. Update a Todo");
        Console.WriteLine("5. Delete a Todo");
        Console.WriteLine("100. Close Todo interface");
        choice = int.Parse(Console.ReadLine());
        TodoDets todo = new();

        switch(choice)
        {
            case 1:
                await AllTodos();
                break;

            case 2:
                await TodoDetails();
                break;

            case 3:
                await CreateTodo();
                break;

            case 4:
                await UpdateTodo();
                break;

            case 5:
                await DeleteTodo();
                break;

            case 100:
                Console.WriteLine("!!!!!! CLIENT CONSOLE IS CLOSED !!!!!!!!");
                keepRunning = false;
                break;

            default:
                Console.WriteLine("!!! Wrong choice please choose again !!!");
                break;
        }
    }
}
else
{
    Console.WriteLine(responseMessage.StatusCode.ToString());
}

Console.ReadKey();

async Task AllTodos()
{
    responseMessage = await client.GetAsync("api/todos/all");
    responseMessage.EnsureSuccessStatusCode();
    var todos = await responseMessage.Content.ReadFromJsonAsync<IEnumerable<TodoDets>>();

    foreach (var todo in todos)
    {
        Console.WriteLine(todo.Id + ". " + todo.Title);
    }

}

async Task TodoDetails()
{
    await AllTodos();

    Console.Write("Choose a todo by id to view it's details: ");
    int id = int.Parse(Console.ReadLine());
    responseMessage = await client.GetAsync($"api/todos/details/{id}");
    responseMessage.EnsureSuccessStatusCode();
    var todo = await responseMessage.Content.ReadFromJsonAsync<TodoDets>();

    Console.WriteLine("ID: " + todo.Id +
                "\nTitle: " + todo.Title +
                "\nDescription: " + todo.Description +
                "\nDone: " + todo.Done +
                "\nPriority: " + todo.Priority +
                "\nCreated: " + todo.Created);
}

async Task CreateTodo()
{
    Console.WriteLine("-----------Creating todo------------");
    int input = 0;
    TodoDets todo = new();

    Console.Write("Enter issue title: ");
    todo.Title = Console.ReadLine();

    Console.Write("Enter issue description: ");
    todo.Description = Console.ReadLine();

    Console.Write("Enter issue priority(1 = Low, 2 = Medium, 3 = High): ");
    input = int.Parse(Console.ReadLine());
    todo.Priority = input - 1 == 0 ? Priority.Low
        : input - 1 == 1 ? Priority.Medium
        : Priority.High;

    todo.Done = false;
    todo.Created = DateTime.Now;

    var stringPayload = JsonConvert.SerializeObject(todo);
    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

    responseMessage = await client.PostAsync("api/todos/Create", httpContent);
    responseMessage.EnsureSuccessStatusCode();


    Console.WriteLine("!!! Todo Created !!!");
}

async Task DeleteTodo()
{
    await AllTodos();

    Console.WriteLine();
    Console.Write("Select todo to delete using id: ");
    int input = int.Parse(Console.ReadLine());

    responseMessage = await client.DeleteAsync($"api/todos/Delete/{input}");
    responseMessage.EnsureSuccessStatusCode();

    Console.WriteLine("!!!! Issue deleted !!!");

    Console.WriteLine();
}

async Task UpdateTodo()
{
    await AllTodos();
    char answer = ' ';
    int detail = 0;

    Console.Write("Choose todo to update by id: ");
    int id = int.Parse(Console.ReadLine());

    responseMessage = await client.GetAsync($"api/todos/details/{id}");
    responseMessage.EnsureSuccessStatusCode();
    var todo = await responseMessage.Content.ReadFromJsonAsync<TodoDets>();

    Console.Write("Update todo title?(y/n): ");
    answer = char.Parse(Console.ReadLine());
    if (answer == 'y')
    {
        Console.Write("Enter todo title: ");
        todo.Title = Console.ReadLine();
    }

    Console.Write("Update todo description?(y/n): ");
    answer = char.Parse(Console.ReadLine());
    if (answer == 'y')
    {
        Console.Write("Enter todo description: ");
        todo.Description = Console.ReadLine();
    }

    Console.Write("Update todo done status?(y/n): ");
    answer = char.Parse(Console.ReadLine());
    if (answer == 'y')
    {
        Console.Write("Enter todo done status(true/false): ");
        todo.Done = bool.Parse(Console.ReadLine());
    }

    Console.Write("Update todo priority?(y/n): ");
    answer = char.Parse(Console.ReadLine());
    if (answer == 'y')
    {
        Console.Write("Enter item department(1 = Low, 2 = Medium, 3 = High: ");
        detail = int.Parse(Console.ReadLine());
        todo.Priority = detail - 1 == 0 ? Priority.Low
                    : detail - 1 == 1 ? Priority.Medium
                    : Priority.High;
    }


    var payload = JsonConvert.SerializeObject(todo);
    var content = new StringContent(payload, Encoding.UTF8, "application/json");

    responseMessage = await client.PutAsync($"api/todos/update/{id}", content);
    responseMessage.EnsureSuccessStatusCode();

    Console.WriteLine("!!! Todo updated successfully !!!");
}