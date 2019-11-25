using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using MLNET.RealWorld.Services;

namespace MLNET.RealWorld.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassifierController : ControllerBase
    {
        [HttpGet, Route(nameof(TrainSpamDetector))]
        public async Task<IActionResult> TrainSpamDetector([FromServices] SpamDetectorTrainer spamDetectorTrainer) {
            await spamDetectorTrainer.Train();
            return Ok();
        }

        [HttpGet, Route(nameof(GetClassifierAsZip))]
        public async Task<IActionResult> GetClassifierAsZip([FromServices] DbModelManager dbModelManager, [FromServices] MLContext mlContext) {
            var trainedModel = await dbModelManager.LoadLast();
            var ms = new MemoryStream(trainedModel.ModelData);
            return File(ms, "application/zip");
        }
    }
}
