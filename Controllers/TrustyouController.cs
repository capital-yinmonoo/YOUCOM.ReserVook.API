using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TrustyouController : ControllerBase
    {
        private readonly ILogger<TrustyouController> _logger;
        private ITrustyouService _trustyouService;

        public TrustyouController(ILogger<TrustyouController> logger, ITrustyouService trustyouService)
        {
            _logger = logger;

            _trustyouService = trustyouService;

        }

        [HttpPost("getTrustyouList")]
        public async Task<List<TrustyouInfo>> GetTrustyouList(TrustyouCondition cond){
            return await _trustyouService.GetTrustyouList(cond);
        }

        [HttpPost("sendRecvTrustyouData")]
        public async Task<List<TrnTrustyouLogInfo>> SendRecvTrustyouData(TrustyouSendRcvCondition sendRecvCond) {
            return await _trustyouService.SendRecvTrustyouData(sendRecvCond);
        }

        [HttpPost("getTrustyouLogList")]
        public async Task<List<TrnTrustyouLogInfo>> GetTrustyouLogList(TrustyouLogCondition cond) {
            return await _trustyouService.GetTrustyouLogList(cond);
        }

        [HttpPost("saveTemporarilyData")]
        public async Task<int> SaveTemporarilyData(List<TrustyouInfo> tempList) {
            return await _trustyouService.SaveTemporarilyData(tempList);
        }
    }
}
