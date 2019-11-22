using Microsoft.ML.Data;

namespace MLNET.Core {
    public class SpamInput
    {
        [LoadColumn(0), ColumnName("Label")]
        public string Label { get; set; }

        [LoadColumn(1)]
        public string Message { get; set; }
    }
}