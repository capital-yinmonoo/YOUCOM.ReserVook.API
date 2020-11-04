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
    public class CleaningListController : ControllerBase
    {
        private readonly ILogger<CleaningListController> _logger;
        private IRoomsService _roomsService;

        public CleaningListController(ILogger<CleaningListController> logger, IRoomsService roomsService)
        {
            _logger = logger;

            _roomsService = roomsService;

        }

        [HttpPost("getCleaningsList")]
        public async Task<List<CleaningInfo>> GetCleaningList(CleaningCondition cond)
        {
            return await _roomsService.GetCleaningList(cond);
        }


        /// <summary>
        /// 客室状態更新 更新
        /// </summary>
        /// <param name="assignInfo"></param>
        /// <returns></returns>
        [HttpPut("updateRoomStatus")]
        public async Task<CommonEnum.DBUpdateResult> UpdateRoomStatus(TrnReserveAssignInfo assignInfo) {
            try {
                int result = await _roomsService.UpdateRoomStatus(assignInfo);
                if (result == 1) {
                    return CommonEnum.DBUpdateResult.Success;
                } else if (result == -1) {
                    return CommonEnum.DBUpdateResult.VersionError;
                } else {
                    return CommonEnum.DBUpdateResult.Error;
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }
    }
}
