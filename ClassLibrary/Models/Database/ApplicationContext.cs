using Microsoft.EntityFrameworkCore;
using DudesPlayer.Library.Models;
using System.Data.Common;

public class ApplicationContext : DbContext
{
    public DbSet<RoomInfo> Rooms => Set<RoomInfo>();
    public DbSet<URLModel> URLs => Set<URLModel>();
    public DbSet<RoomSettings> Settings => Set<RoomSettings>();
    public DbSet<EventInfo> Events => Set<EventInfo>();
    public ApplicationContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=postgres;Password=password;CommandTimeout=6000"); //"Server=(localdb)\\mssqllocaldb;Initial Catalog=DB1;Database=aboba1;Trusted_Connection=True;");
    }
}