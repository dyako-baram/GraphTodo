using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();
    

var app = builder.Build();


app.MapGraphQL();

app.Run();

public class Query
{
    public async Task<List<Todo>?> GetTodo([Service] TodoDb db)
    {
        return await db.Todos.AsNoTracking().ToListAsync();
    }
    public async Task<Todo?> GetTodoById([Service] TodoDb db,int id)
    {
        var result=await db.Todos.FirstOrDefaultAsync(x=>x.Id==id);
        if (result is null)
            return null;
        return result;
    }
    public async Task<List<Todo>?> GetTodoById([Service] TodoDb db,string title)
    {
        var result=await db.Todos.AsNoTracking().Where(x=>x.Title.Contains(title)).ToListAsync();
        if (result is null)
                return null;
        return result;
    }
}
public class Mutation
{
    public async Task<Todo> AddTodo([Service] TodoDb db,Todo todo)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return todo;
    }
}
public class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsComplete { get; set; }
}
public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }
    
    public DbSet<Todo> Todos => Set<Todo>();
    
}