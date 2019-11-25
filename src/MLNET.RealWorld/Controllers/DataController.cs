using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLNET.RealWorld.Db;

namespace MLNET.RealWorld.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        [HttpGet, Route(nameof(GetAllRecords)), ProducesResponseType(typeof(List<SpamRecord>), 200)]
        public async Task<ActionResult<List<SpamRecord>>> GetAllRecords([FromServices] SpamDetectorDbContext context) => await context.SpamRecords.ToListAsync();
    }
}
