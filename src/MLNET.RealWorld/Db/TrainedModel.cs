using System;

namespace MLNET.RealWorld.Db {
    public class TrainedModel
    {
        public int Id { get; set; }
        public virtual byte[] ModelData { get; set; }
        public double Accuracy { get; set; }
        public DateTime TrainedAt { get; set; } = DateTime.UtcNow;
    }
}