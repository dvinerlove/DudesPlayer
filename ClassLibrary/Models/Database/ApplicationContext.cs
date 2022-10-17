using Microsoft.EntityFrameworkCore;
using DudesPlayer.Library.Models;
using System.Data.Common;
using DudesPlayer.Library.Models.Database;

public class ApplicationContext : DbContext
{
    public DbSet<RoomInfo> Rooms => Set<RoomInfo>();
    public DbSet<URLModel> URLs => Set<URLModel>();
    public DbSet<RoomSettings> Settings => Set<RoomSettings>();
    public DbSet<EventInfo> Events => Set<EventInfo>();
    public ApplicationContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Connection.String); //"Server=(localdb)\\mssqllocaldb;Initial Catalog=DB1;Database=aboba1;Trusted_Connection=True;");
    }
}