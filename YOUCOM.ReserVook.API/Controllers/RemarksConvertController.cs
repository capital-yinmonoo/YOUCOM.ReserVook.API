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
    public class RemarksConvertController : ControllerBase
    {


        private IRemarksConvertService _remarksConvertService;


        public RemarksConvertController(IRemarksConvertService remarksConvertService)
        {
            _remarksConvertService = remarksConvertService;

        }

        // 情報取得(画面表示用)
        [HttpPost("getList")]
        public async Task<List<FrMScRemarksConvertInfo>> GetRemarksConvertList(FrMScRemarksConvertInfo remarksConvertInfo)
        {
            var result = await _remarksConvertService.GetRemarksConvertList(remarksConvertInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getListById")]
        public async Task<FrMScRemarksConvertInfo> GetRemarksConvertById(FrMScRemarksConvertInfo remarksConvertInfo)
        {
            return await _remarksConvertService.GetRemarksConvertById(remarksConvertInfo);
        }

        // 情報更新
        [HttpPut("updateRemarksConvert")]
        public async Task<CommonEnum.DBUpdateResult> UpdateRemarksConvert(FrMScRemarksConvertInfo remarksConvertInfo)
        {
            try
            {
                int result = await _remarksConvertService.UpdateRemarksConvert(remarksConvertInfo);
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