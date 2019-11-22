using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MLNET.RealWorld.Services;

namespace MLNET.RealWorld.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassificationController : ControllerBase
    {
        [HttpGet, Route(nameof(TrainSpamDetector))]
        public async Task<IActionResult> TrainSpamDetector([FromServices] SpamDetectorTrainer spamDetectorTrainer) {
            await spamDetectorTrainer.Train();
            return Ok();
        }
    }
}
