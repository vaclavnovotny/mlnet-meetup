using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLNET.RealWorld.Db;

namespace MLNET.RealWorld.Services {
    public class DbModelManager
    {
        private readonly SpamDetectorDbContext _context;

        public DbModelManager(SpamDetectorDbContext context) {
            _context = context;
        }

        public async Task Save(ITransformer bestRunModel, MLContext mlContext, DataViewSchema schema, MulticlassClassificationMetrics metrics) {
            await using var ms = new MemoryStream();
            mlContext.Model.Save(bestRunModel, schema, ms);

            var trainedModel = new TrainedModel() { ModelData = ms.ToArray(), Accuracy = metrics.MicroAccuracy};

            _context.TrainedModels.Add(trainedModel);
            await _context.SaveChangesAsync();
        }

        public async Task<TrainedModel> LoadLast() {
            return await _context.TrainedModels.OrderByDescending(x => x.TrainedAt).FirstAsync();
        }
    }
}