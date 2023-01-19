using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FreelanceApi2;
using FreelanceApi2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ReceiptsContext>(builder.Configuration.GetConnectionString("cnReceipts"));

builder.Services.AddCors(options =>
{
    // options.AddDefaultPolicy(policy =>
    // {
    //     policy.WithOrigins("http://127.0.0.1:5173/");
    // });
    options.AddPolicy("NewPolicy", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/dbconexion", ([FromServices] ReceiptsContext dbContext) =>
{
    dbContext.Database.EnsureCreated();
    return Task.FromResult(Results.Ok("Base de datos en memoria: " + dbContext.Database.IsInMemory()));

});

app.MapGet("/api/receipts", ([FromServices] ReceiptsContext dbContext) =>
{
    return Task.FromResult(Results.Ok(dbContext.Receipts));

});


app.MapPost("/api/receipts", async ([FromServices] ReceiptsContext dbContext, [FromBody] Receipt receipt) =>
{
    receipt.ReceiptId = Guid.NewGuid();
    receipt.CreateAt = DateTime.Now;
    await dbContext.AddAsync(receipt);
    //await dbContext.Tareas.AddAsync(receipt);

    await dbContext.SaveChangesAsync();

    return Results.Ok(receipt);
});

app.MapDelete("/api/receipts/{id}", async ([FromServices] ReceiptsContext dbContext, [FromRoute] Guid id) =>
{
    var currentRecipt = dbContext.Receipts.Find(id);

    if (currentRecipt != null)
    {
        dbContext.Remove(currentRecipt);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }

    return Results.NotFound();
});

app.UseCors("NewPolicy");


app.Run();
