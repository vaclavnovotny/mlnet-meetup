using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLNET.RealWorld.Db {
    public class TrainedModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual byte[] ModelData { get; set; }
        public double Accuracy { get; set; }
        public DateTime TrainedAt { get; set; } = DateTime.UtcNow;
    }
}