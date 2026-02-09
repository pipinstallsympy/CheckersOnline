using CheckersOnline;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

builder.Services.AddSingleton<QuickGameQueue>();
builder.Services.AddSingleton<GameDictionary>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapHub<GameHub>("/gamehub");

app.UseDefaultFiles();
app.UseStaticFiles();



app.MapGet("/status", () => new { Status = "Server is running", Time = DateTime.Now });
app.MapPost("/make-move", async (string move, IHubContext<GameHub> hubContext) =>
{
    await hubContext.Clients.All.SendAsync("Receive move", $"Server processed: {move}");
    return Results.Ok(new {Message = "Move send"});
});


app.Run();