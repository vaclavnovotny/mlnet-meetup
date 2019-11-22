using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.AutoML;

namespace MLNET.RealWorld.Services
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

            var dataProcessPipeline =
                    _mlContext.Transforms.Text.FeaturizeText("Features", "Message")
                    .Append(_mlContext.Transforms.NormalizeLpNorm("Features", "Features"))
                    .AppendCacheCheckpoint(_mlContext);

            var transformerChain = dataProcessPipeline.Fit(data);
            var trainData = transformerChain.Transform(data);

            var experiment = _mlContext
                .Auto()
                .CreateBinaryClassificationExperiment(20)
                .Execute(trainData);
            var bestRunModel = experiment.BestRun.Model;

            var predictions = bestRunModel.Transform(trainData);
            var metrics = _mlContext.BinaryClassification.EvaluateNonCalibrated(predictions);

            await _dbModelManager.Save(bestRunModel, _mlContext, trainData.Schema, metrics);
        }
    }
}
