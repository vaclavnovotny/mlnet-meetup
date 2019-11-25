using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MLNET.RealWorld.Models;
using MLNET.RealWorld.Services;

namespace MLNET.RealWorld.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        [HttpGet, Route(nameof(IsSpam))]
        [ProducesResponseType(typeof(SpamPrediction), 200)]
        public async Task<ActionResult<bool>> IsSpam([Required] string message, [FromServices] SpamDetector spamDetector) {
            await Task.CompletedTask;
            var spamPrediction = spamDetector.Predict(message);
            return Ok(spamPrediction);
        }
    }
}