using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [Route("[controller]")]
    [ApiController]
    public class RoomInfoController : ControllerBase
    {

        private IRoomInfoService _roomInfoService;

        public RoomInfoController(IRoomInfoService roomInfoService)
        {
            _roomInfoService = roomInfoService;
        }

        // 情報取得
        [HttpPost("getInfo")]
        public async Task<MstRoomsInfo> GetRoomInfo(MstRoomsInfo roomInfo)
        {
            return await _roomInfoService.GetRoomInfo(roomInfo);
        }

        // 更新
        [HttpPut("updateRoomInfo")]
        public async Task<CommonEnum.DBUpdateResult> UpdateRoomInfo(MstRoomsInfo roomInfo)
        {
            try
            {
                int result = await _roomInfoService.UpdateRoomInfo(roomInfo);
                if (result == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (result == -1)
                {
                    return CommonEnum.DBUpdateResult.VersionError;
                }
                else
                {
                    return CommonEnum.DBUpdateResult.Error;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }
    }
}