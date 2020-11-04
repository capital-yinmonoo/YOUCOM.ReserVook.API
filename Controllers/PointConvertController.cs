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
    public class PointConvertController : ControllerBase
    {


        private IPointConvertService _pointConvertService;


        public PointConvertController(IPointConvertService pointConvertService)
        {
            _pointConvertService = pointConvertService;

        }

        // 情報取得(画面表示用)
        [HttpPost("getList")]
        public async Task<List<FrMScPointConvertInfo>> GetPointConvertList(FrMScPointConvertInfo pointConvertInfo)
        {
            var result = await _pointConvertService.GetPointConvertList(pointConvertInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getListById")]
        public async Task<FrMScPointConvertInfo> GetPointConvertById(FrMScPointConvertInfo pointConvertInfo)
        {
            return await _pointConvertService.GetPointConvertById(pointConvertInfo);
        }

        // 情報追加
        [HttpPost("addPointConvert")]
        public async Task<int> AddPointConvert(FrMScPointConvertInfo pointConvertInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _pointConvertService.AddPointConvert(pointConvertInfo);
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
        [HttpPut("updatePointConvert")]
        public async Task<CommonEnum.DBUpdateResult> UpdatePointConvert(FrMScPointConvertInfo pointConvertInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _pointConvertService.UpdatePointConvert(pointConvertInfo, addFlag);
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
        [HttpPut("delPointConvert")]
        public async Task<CommonEnum.DBUpdateResult> DelPointConvert(FrMScPointConvertInfo pointConvertInfo)
        {
            try
            {
                int ret = await _pointConvertService.DelPointConvert(pointConvertInfo);
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