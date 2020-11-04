using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class LedgerController : ControllerBase
    {
        private readonly ILogger<LedgerController> _logger;
        private ILedgerService _ledgerService;

        public LedgerController(ILogger<LedgerController> logger, ILedgerService ledgerService)
        {
            _logger = logger;

            _ledgerService = ledgerService;

        }

        [HttpPost("getLedgerReport")]
        public async Task<List<LedgerInfo>> GetLedgerReport(LedgerInfo cond)
        {
            return await _ledgerService.GetList(cond);
        }

    }
}
