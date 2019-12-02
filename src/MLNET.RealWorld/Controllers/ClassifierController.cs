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
        /// <summary>
        /// Trains new spam detector classifier.
        /// </summary>
        /// <param name="spamDetectorTrainer"></param>
        /// <returns></returns>
        [HttpGet, Route(nameof(TrainSpamDetector))]
        public async Task<IActionResult> TrainSpamDetector([FromServices] SpamDetectorTrainer spamDetectorTrainer) {
            await spamDetectorTrainer.Train();
            return Ok();
        }

        /// <summary>
        /// Returns latest trained spam detector classifier in form of zip file. Internally used for prediction engine pool.
        /// </summary>
        /// <param name="dbModelManager"></param>
        /// <returns></returns>
        [HttpGet, Route(nameof(GetClassifierAsZip))]
        public async Task<IActionResult> GetClassifierAsZip([FromServices] DbModelManager dbModelManager) {
            var trainedModel = await dbModelManager.LoadLast();
            return File(trainedModel.ModelData, "application/zip");
        }
    }
}
