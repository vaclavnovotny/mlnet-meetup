using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.AutoML;

namespace MLNET.SpamDetector.RealWorld.Services
{
    public class SpamDetectorTrainer
    {
        private readonly MLContext _mlContext;
        private readonly DataLoader _dataLoader;
        private readonly DbModelManager _dbModelManager;

        public SpamDetectorTrainer(MLContext mlContext, DataLoader dataLoader, DbModelManager dbModelManager) {
            _mlContext = mlContext;
            _dataLoader = dataLoader;
            _dbModelManager = dbModelManager;
        }

        public async Task Train() {
            var data = await _dataLoader.Load();

            var dataProcessPipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Message")
                .Append(_mlContext.Transforms.NormalizeLpNorm("Features", "Features"))
                .AppendCacheCheckpoint(_mlContext);

            var experiment = _mlContext
                .Auto()
                .CreateMulticlassClassificationExperiment(new MulticlassExperimentSettings() {
                    OptimizingMetric = MulticlassClassificationMetric.MicroAccuracy, 
                    MaxExperimentTimeInSeconds = 120
                })
                .Execute(data, "IsSpam", preFeaturizer: dataProcessPipeline);

            var bestRunModel = experiment.BestRun.Model;
            var predictions = bestRunModel.Transform(data);
            var metrics = _mlContext.MulticlassClassification.Evaluate(predictions, "IsSpam");

            await _dbModelManager.Save(bestRunModel, _mlContext, data.Schema, metrics);
        }
    }
}
