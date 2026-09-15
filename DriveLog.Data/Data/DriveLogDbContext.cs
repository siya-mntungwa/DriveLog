using Microsoft.EntityFrameworkCore;

namespace DriveLog.Data.Data;

public class DriveLogDbContext : DbContext {
    public DriveLogDbContext(DbContextOptions<DriveLogDbContext> options) : base(options) {
        
    }
}