﻿using Microsoft.ML.Data;

namespace MLNET.RealWorld.Models {
    public class SpamPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsSpam { get; set; }
        [ColumnName("Score"), VectorType(2)]
        public float[] Score { get; set; }
    }
}