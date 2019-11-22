using System.ComponentModel.DataAnnotations;
using Microsoft.ML.Data;

namespace MLNET.RealWorld.Db {
    public class SpamRecord
    {
        [Key]
        public int Id { get; set; }
        [ColumnName("Label")]
        public bool IsSpam { get; set; }
        public string Message { get; set; }
    }
}