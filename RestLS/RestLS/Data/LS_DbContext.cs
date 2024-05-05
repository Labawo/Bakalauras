using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestLS.Auth.Models;
using RestLS.Data.Entities;

namespace RestLS.Data;

public class LS_DbContext : IdentityDbContext<ClinicUser>
{
    protected readonly IConfiguration Configuration;

    public LS_DbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to mysql with connection string from app settings
        var connectionString = Configuration.GetConnectionString("WebApiDatabase");
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Recomendation> Recomendations { get; set; }
    public DbSet<Therapy> Therapies { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=AppointmentDB2");
    }*/
    
}