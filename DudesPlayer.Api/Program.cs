using DudesPlayer.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
if (app.Environment.IsDevelopment())
{
}

    app.UseSwagger();
    app.UseSwaggerUI();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
 
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
});
try
{
    SocketServer.Start();
    WSServer.Start();
    Cleaner.StartPing();
    Cleaner.StartCleanUsers();
}
catch
{

}

app.Run();

