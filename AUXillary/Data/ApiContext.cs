using AUXillary.Models;
using Microsoft.EntityFrameworkCore;

namespace AUXillary.Data;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}