using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models;
using System.Data.Common;
using ClassLibrary.Models.Database;

public class ApplicationContext : DbContext
{
    public DbSet<RoomInfo> Rooms => Set<RoomInfo>();
    public DbSet<URLModel> URLs => Set<URLModel>();
    public DbSet<RoomSettings> Settings => Set<RoomSettings>();
    public DbSet<EventInfo> Events => Set<EventInfo>();
    public ApplicationContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Connection.String);
    }
}