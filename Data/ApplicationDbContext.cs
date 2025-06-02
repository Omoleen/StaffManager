using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaffManagementN.Models;

namespace StaffManagementN.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<EmployeeModel> Employees { get; set; }
    
    public DbSet<ShiftModel> Shifts { get; set; }
}