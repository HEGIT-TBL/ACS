using Microsoft.EntityFrameworkCore;
using ACS.Core.Models;
using Microsoft.Extensions.Configuration;
using ACS.Core.Models.Events;

namespace ACS.Core.Data
{
    public class AccessControlDbContext : DbContext
    {
        #region uncomment & set ACS.Core as startup project to use EFCore tools and generate new .dmgl
        //public AccessControlDbContext()
        //    : base()
        //{ }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=ACS_DB;User Id=Admin;Password=MyCoolP@$$w0rd");
        //}
        #endregion

        public AccessControlDbContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<AccessEvent> AccessEvents { get; set; }
        public DbSet<AccessPoint> AccessPoints { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<FaceRecognizedEvent> FaceRecognizedEvents { get; set; }
        public DbSet<Identifier> Identifiers { get; set; }
        public DbSet<KeyCard> KeyCards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<ParkingLot> ParkingLots { get; set; }
        public DbSet<ParkingLotStateChangedEvent> ParkingLotStateChangedEvents { get; set; }
    }
}