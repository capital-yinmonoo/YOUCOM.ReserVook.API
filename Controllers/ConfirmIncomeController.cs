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
    public class ConfirmIncomeController : ControllerBase
    {
        private readonly ILogger<ConfirmIncomeController> _logger;
        private IConfirmIncomeService _confirmIncomeInfoService;

        public ConfirmIncomeController(ILogger<ConfirmIncomeController> logger, IConfirmIncomeService confirmIncomeService)
        {
            _logger = logger;

            _confirmIncomeInfoService = confirmIncomeService;

        }

        //查询管理员List
        [HttpPost("getIncomeList")]
        public async Task<List<ConfirmIncomeInfo>> GetIncomeList(incomeQuery cond)
        {
            return await _confirmIncomeInfoService.GetIncomeList(cond.companyCode, cond.queryDate);
        }

    }
}
