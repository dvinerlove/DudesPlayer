
using AspWebServer.Controllers;
using AspWebServer.Models;




try
{
    //SocketServer.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

var builder = WebApplication.CreateBuilder(args);

//AspWebServer.Models.ServerEventHandler.CheckUsers();
//AspWebServer.Models.ServerEventHandler.Ping();
// Add services to the container.

builder.Services.AddRazorPages();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
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

