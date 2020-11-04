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
    //[EnableCors("AllowSpecificOrigin")]
    [Route("[controller]")]
    [ApiController]
    public class RoomTypeConvertController : ControllerBase
    {


        private IRoomTypeConvertService _roomTypeConvertService;


        public RoomTypeConvertController(IRoomTypeConvertService roomTypeConvertService)
        {
            _roomTypeConvertService = roomTypeConvertService;

        }

        // 情報取得(画面表示用)
        [HttpPost("getlist")]
        public async Task<List<FrMScRmtypeConvertInfo>> GetRoomTypeConvertList(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            var result = await _roomTypeConvertService.GetRoomTypeConvertList(roomTypeConvertInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getlistbyid")]
        public async Task<FrMScRmtypeConvertInfo> GetRoomTypeConvertById(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            return await _roomTypeConvertService.GetRoomTypeConvertById(roomTypeConvertInfo);
        }

        // 情報追加
        [HttpPost("addroomtypeconvert")]
        public async Task<int> AddRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _roomTypeConvertService.AddRoomTypeConvert(roomTypeConvertInfo);
                if (result == 0)
                {
                    return addFlg;
                }
                else
                {
                    addFlg = 1;
                    return addFlg;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                addFlg = -1;
                return addFlg;
            }

        }

        // 情報更新
        [HttpPut("updateroomtypeconvert")]
        public async Task<CommonEnum.DBUpdateResult> UpdateRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _roomTypeConvertService.UpdateRoomTypeConvert(roomTypeConvertInfo, addFlag);
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

        // 情報削除
        [HttpPut("delroomtypeconvert")]
        public async Task<CommonEnum.DBUpdateResult> DelRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            try
            {
                int ret = await _roomTypeConvertService.DelRoomTypeConvert(roomTypeConvertInfo);
                if (ret == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (ret == -1)
                {
                    return CommonEnum.DBUpdateResult.VersionError;
                }
                else if (ret == -2)
                {
                    return CommonEnum.DBUpdateResult.UsedError;
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