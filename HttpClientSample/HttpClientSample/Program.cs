using HttpClientSample;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

class Program
{
    static HttpClient client = new HttpClient();

    static void ShowTodo(TodoItem todo)
    {
        Console.WriteLine($"Name: {todo.Name}\tIsComplete: " +
            $"{todo.IsComplete}");
    }

    static async Task<Uri> CreateTodoAsync(TodoItem todo)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "api/TodoItems", todo);
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return response.Headers.Location;
    }

    static async Task<TodoItem> GetTodoAsync(string path)
    {
        TodoItem todo = null;
        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            todo = await response.Content.ReadAsAsync<TodoItem>();
        }
        return todo;
    }

    static async Task<TodoItem> UpdateTodoAsync(TodoItem todo)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"api/TodoItems/{todo.Id}", todo);
        response.EnsureSuccessStatusCode();

        // Deserialize the updated todo from the response body.
        todo = await response.Content.ReadAsAsync<TodoItem>();
        return todo;
    }

    static async Task<HttpStatusCode> DeleteTodoAsync(long id)
    {
        HttpResponseMessage response = await client.DeleteAsync(
            $"api/TodoItems/{id}");
        return response.StatusCode;
    }

    static void Main()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    static async Task RunAsync()
    {
        // Update port # in the following line.
        client.BaseAddress = new Uri("https://localhost:7201/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            // Create a new todo
            TodoItem todo = new TodoItem
            {
                Name = "walk dog",
                IsComplete = true
            };

            var url = await CreateTodoAsync(todo);
            Console.WriteLine($"Created at {url}");

            // Get the todo
            todo = await GetTodoAsync(url.PathAndQuery);
            ShowTodo(todo);

            // Update the todo
            Console.WriteLine("Updating price...");
            todo.IsComplete = true;
            await UpdateTodoAsync(todo);

            // Get the updated todo
            todo = await GetTodoAsync(url.PathAndQuery);
            ShowTodo(todo);

            // Delete the todo
            var statusCode = await DeleteTodoAsync(todo.Id);
            Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadLine();
    }
}