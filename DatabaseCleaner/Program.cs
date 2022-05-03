// See https://aka.ms/new-console-template for more information
using DatabaseCleaner;

Console.WriteLine("Hello, World!");
Cleaner.StartPing();
Cleaner.StartCleanUsers();
while (true)
{
    await Task.Delay(10000);
}