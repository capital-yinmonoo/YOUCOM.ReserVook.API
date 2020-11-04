using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ReserveLogController : ControllerBase
    {

        private IReserveLogService _reserveLogService;

        public ReserveLogController(IReserveLogService reserveLogService)
        {
            _reserveLogService = reserveLogService;

        }

        /// <summary>
        /// 予約変更履歴 一覧取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getReserveLogList")]
        public async Task<List<TrnReserveLogInfo>> GetReserveLogList(TrnReserveLogInfo cond)
        {
            var result = await _reserveLogService.GetReserveLogList(cond);
            return result;
        }
    }
}