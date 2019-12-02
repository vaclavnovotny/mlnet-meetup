using Microsoft.Extensions.ML;
using MLNET.SpamDetector.RealWorld.Models;

namespace MLNET.SpamDetector.RealWorld.Services
{
    public class SpamDetector
    {
        private readonly PredictionEnginePool<SpamInput, SpamPrediction> _predictionEnginePool;
        public SpamDetector(PredictionEnginePool<SpamInput, SpamPrediction> predictionEnginePool) {
            _predictionEnginePool = predictionEnginePool;
        }

        public SpamPrediction Predict(string message) => _predictionEnginePool.Predict(ModelsCatalog.SpamDetector, new SpamInput() {Message = message});
    }
}
