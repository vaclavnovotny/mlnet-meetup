using Microsoft.Extensions.ML;
using MLNET.RealWorld.Models;

namespace MLNET.RealWorld.Services
{
    public class SpamDetector
    {
        private readonly PredictionEnginePool<SpamInput, SpamPrediction> _predictionEnginePool;
        public SpamDetector(PredictionEnginePool<SpamInput, SpamPrediction> predictionEnginePool) {
            _predictionEnginePool = predictionEnginePool;
        }

        public SpamPrediction Predict(string message) => _predictionEnginePool.Predict("SpamDetector", new SpamInput() {Message = message});
    }
}
