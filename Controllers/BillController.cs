using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers
{
    //     [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BillController : ControllerBase
    {
        private readonly ILogger<BillController> _logger;
        private IBillService _billService;


        public BillController(ILogger<BillController> logger, IBillService billService)
        {
            _logger = logger;

            _billService = billService;

        }

        /// <summary>
        /// 予約に存在するビル分割Noのリストを取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getSeparateBillNoList")]
        public async Task<IList<string>> GetSeparateBillNoList(ReserveModel cond)
        {
            return await _billService.GetSeparateBillNoList(cond);
        }


        [HttpPost("checkBillNo")]
        public async Task<CommonEnum.DBUpdateResult> CheckBillNo(ReserveModel cond)
        {
            if (await _billService.CheckBillNo(cond))
            {
                return CommonEnum.DBUpdateResult.Success;
            }
            else
            {
                return CommonEnum.DBUpdateResult.Error;
            }

        }

    }
}