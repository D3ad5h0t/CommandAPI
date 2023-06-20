using CommandAPI.Data;
using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLDbConnection")));
builder.Services.AddScoped<ICommandRepo, SqlCommandRepo>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//GET All Commands
app.MapGet("api/v1/commands", async (ICommandRepo repo) =>
{
    var commands = await repo.GetAllCommandsAsync();

    return Results.Ok(commands);
});

//GET Single
app.MapGet("api/v1/commands/{commandId}", async (ICommandRepo repo, string commandId) =>
{
    var command = await repo.GetCommandByIdAsync(commandId);

    if (command != null)
    {
        return Results.Ok(command);
    }

    return Results.NotFound();
});

//Create
app.MapPost("api/v1/commands", async (ICommandRepo repo, Command cmd) =>
{
    await repo.CreateCommandAsync(cmd);
    await repo.SaveChangesAsync();

    return Results.Created($"/api/v1/commands/{cmd.CommandId}", cmd);
});

//Update
app.MapPut("api/v1/commands/{commandId}", async (ICommandRepo repo, string commandId, Command cmd) =>
{
    var command = await repo.GetCommandByIdAsync(commandId);

    if (command is null) return Results.NotFound();

    command.HowTo = cmd.HowTo;
    command.CommandLine = cmd.CommandLine;
    command.Platform = cmd.Platform;

    await repo.SaveChangesAsync();

    return Results.NoContent();
});

//Delete
app.MapDelete("api/v1/commands/{commandId}", async (ICommandRepo repo, string commandId) =>
{
    var command = await repo.GetCommandByIdAsync(commandId);

    if (command is null) return Results.NotFound();

    repo.DeleteCommand(command);
    await repo.SaveChangesAsync();

    return Results.Ok(command);
});

app.Run();
