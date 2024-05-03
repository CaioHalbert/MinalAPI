using Microsoft.EntityFrameworkCore;
using MinalAPI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(o => o.UseInMemoryDatabase("Todolist"));

var app = builder.Build();

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitem/{todo.Id}", todo);
});

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todos.Where(x => x.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>

    await db.Todos.FindAsync(id)
        is Todo todo ? Results.Ok(todo) : Results.NotFound());

app.MapPut("/todoitems/{id}", async(int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if(todo is null) return Results.NotFound();
    todo.Title = inputTodo.Title;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if(await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok("Deletado");
    }
    return Results.NotFound();
});

app.Run();
