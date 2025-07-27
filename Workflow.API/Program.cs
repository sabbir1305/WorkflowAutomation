using Microsoft.EntityFrameworkCore;
using Workflow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WorkflowDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/workflows", async (WorkflowDbContext db) => await db.Workflows.ToListAsync());
app.MapPost("/workflows", async (WorkflowDbContext db, Workflow.Core.Models.WorkflowDefinition wf) =>
{
    db.Workflows.Add(wf);
    await db.SaveChangesAsync();
    return Results.Created($"/workflows/{wf.Id}", wf);
});

app.MapDelete("/workflows/{id}", async (WorkflowDbContext db, Guid id) =>
{
    var wf = await db.Workflows.FindAsync(id);
    if (wf is null) return Results.NotFound();
    db.Workflows.Remove(wf);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
