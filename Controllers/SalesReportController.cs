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
    public class SalesReportController : ControllerBase
    {

        private readonly ILogger<SalesReportController> _logger;
        private ISalesReportService _salesReportService;

        public SalesReportController(ILogger<SalesReportController> logger, ISalesReportService salesReportService)
        {
            _logger = logger;

            _salesReportService = salesReportService;
        }


        [HttpPost("getSalesReport")]
        public async Task<List<SalesReportInfo>> GetSalesReport(SalesReportCondition cond)
        {
            return await _salesReportService.GetList(cond);
        }
    }
}