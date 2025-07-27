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

// Seed database at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    Workflow.Infrastructure.DbSeeder.SeedAsync(db).GetAwaiter().GetResult();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/workflows", async (WorkflowDbContext db) => await db.Workflows.Include(w => w.Actions).ThenInclude(a => a.Parameters).ToListAsync());
app.MapGet("/workflows/{id}", async (WorkflowDbContext db, Guid id) =>
{
    var wf = await db.Workflows.Include(w => w.Actions).ThenInclude(a => a.Parameters).FirstOrDefaultAsync(w => w.Id == id);
    return wf is not null ? Results.Ok(wf) : Results.NotFound();
});

app.MapPost("/workflows", async (WorkflowDbContext db, Workflow.Core.Models.WorkflowDefinition wf) =>
{
    db.Workflows.Add(wf);
    // Ensure actions and parameters are tracked and saved
    if (wf.Actions is not null)
    {
        foreach (var action in wf.Actions)
        {
            db.Entry(action).State = EntityState.Added;
            if (action.Parameters is not null)
            {
                foreach (var param in action.Parameters)
                {
                    db.Entry(param).State = EntityState.Added;
                }
            }
        }
    }
    await db.SaveChangesAsync();
    return Results.Created($"/workflows/{wf.Id}", wf);
});


app.MapPut("/workflows/{id}", async (WorkflowDbContext db, Guid id, Workflow.Core.Models.WorkflowDefinition updatedWf) =>
{
    var wf = await db.Workflows.Include(x => x.Actions).ThenInclude(a => a.Parameters).FirstOrDefaultAsync(x => x.Id == id);
    if (wf is null) return Results.NotFound();
    var newWf = wf with
    {
        Name = updatedWf.Name,
        TriggerType = updatedWf.TriggerType,
        Actions = updatedWf.Actions
    };
    db.Entry(wf).CurrentValues.SetValues(newWf);
    // Update actions and parameters
    if (updatedWf.Actions is not null)
    {
        wf.Actions.Clear();
        foreach (var action in updatedWf.Actions)
        {
            wf.Actions.Add(action);
            db.Entry(action).State = action.Id == Guid.Empty ? EntityState.Added : EntityState.Modified;
            if (action.Parameters is not null)
            {
                foreach (var param in action.Parameters)
                {
                    db.Entry(param).State = param.Id == Guid.Empty ? EntityState.Added : EntityState.Modified;
                }
            }
        }
    }
    await db.SaveChangesAsync();
    return Results.Ok(newWf);
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
