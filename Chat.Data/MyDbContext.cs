using Chat.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data;

public class MyDbContext : DbContext
{
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite("Data Source=ChatDb.db");

    public MyDbContext()
    {
        this.Database.Migrate();
    }
}