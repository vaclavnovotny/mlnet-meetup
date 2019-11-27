using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MLNET.SpamDetector.RealWorld.Db
{
    public class SpamDetectorDbContext : DbContext
    {
        public SpamDetectorDbContext(DbContextOptions<SpamDetectorDbContext> options) : base(options) { }

        public DbSet<SpamRecord> SpamRecords { get; set; }
        public DbSet<TrainedModel> TrainedModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var trainDataPath = Path.Combine(@"C:\Users\novotnyv\Documents\YSoft\mlnet-meetup\src\MLNET.RealWorld\InitData\SMSSpamCollection");
            var initData = File.ReadAllLines(trainDataPath).Select(x=>x.Split("\t")).Select((x,i)=>new SpamRecord(){Id = i+1, IsSpam = x[0] == "spam", Message = x[1]});
            modelBuilder.Entity<SpamRecord>().HasData(initData);
            base.OnModelCreating(modelBuilder);
        }
    }
}
