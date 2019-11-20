using System;
using Microsoft.ML.Data;

namespace MLNET.SpamDetection {
    public class SpamPrediction
    {
        [ColumnName("PredictedLabel")]
        public string SpamLabel { get; set; }

        [ColumnName("Score"), VectorType(2)]
        public float[] Scores { get; set; }

        public bool IsSpam => SpamLabel.Equals("spam", StringComparison.InvariantCultureIgnoreCase);
    }
}