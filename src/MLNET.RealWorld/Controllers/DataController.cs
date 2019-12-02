using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLNET.SpamDetector.RealWorld.Db;

namespace MLNET.SpamDetector.RealWorld.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        [HttpGet, Route(nameof(GetAllRecords)), ProducesResponseType(typeof(List<SpamRecord>), 200)]
        public async Task<ActionResult<List<SpamRecord>>> GetAllRecords([FromServices] SpamDetectorDbContext context) => Ok(await context.SpamRecords.ToListAsync());

        [HttpPost, Route(nameof(AddSample)), ProducesResponseType(200)]
        public async Task<IActionResult> AddSample([FromServices] SpamDetectorDbContext context, string message, bool isSpam) {
            context.SpamRecords.Add(new SpamRecord() {Message = message, IsSpam = isSpam});
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete, Route(nameof(RemoveSample)), ProducesResponseType(200), ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RemoveSample([FromServices] SpamDetectorDbContext context, int id)
        {
            var spamRecord = await context.SpamRecords.SingleOrDefaultAsync(x => x.Id == id);
            if (spamRecord == null)
                return NotFound($"Spam record with id {id} not found.");
            context.SpamRecords.Remove(spamRecord);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
