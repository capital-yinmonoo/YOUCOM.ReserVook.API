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
    [ApiController]
    [Route("[controller]")]
    public class DishReportController : ControllerBase
    {
        private readonly ILogger<DishReportController> _logger;
        private IDishReportService _dishReportService;

        public DishReportController(ILogger<DishReportController> logger, IDishReportService dishReportService)
        {
            _logger = logger;

            _dishReportService = dishReportService;
        }

        /// <summary>
        /// 料理日報 データ取得
        /// </summary>
        /// <param name="cond">条件</param>
        /// <returns></returns>
        [HttpPost("getList")]
        public async Task<List<DishInfo>> GetList(DishReportCondition cond)
        {
            return await _dishReportService.GetList(cond);
        }

    }
}