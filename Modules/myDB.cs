using Microsoft.EntityFrameworkCore;

namespace MasterCardServer {
      public class myDB : DbContext
    {
        public myDB(DbContextOptions<myDB> options)
            : base(options)
        {
        }

        public DbSet<RoomItem> Rooms { get; set; } 
        public DbSet<Player> Players { get; set; }
    }
}