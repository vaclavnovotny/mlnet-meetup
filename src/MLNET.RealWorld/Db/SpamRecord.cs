using System.ComponentModel.DataAnnotations;

namespace MLNET.RealWorld.Db {
    public class SpamRecord
    {
        [Key]
        public int Id { get; set; }
        public bool IsSpam { get; set; }
        public string Message { get; set; }
    }
}