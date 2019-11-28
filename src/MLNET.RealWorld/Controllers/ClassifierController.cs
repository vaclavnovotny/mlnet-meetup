using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MLNET.SpamDetector.RealWorld.Services;

namespace MLNET.SpamDetector.RealWorld.Controllers
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
        public async Task<IActionResult> GetClassifierAsZip([FromServices] DbModelManager dbModelManager) {
            var trainedModel = await dbModelManager.LoadLast();
            return File(trainedModel.ModelData, "application/zip");
        }
    }
}
