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
    public class PlanConvertController : ControllerBase
    {


        private IPlanConvertService _planConvertService;


        public PlanConvertController(IPlanConvertService planConvertService)
        {
            _planConvertService = planConvertService;

        }

        // 情報取得(画面表示用)
        [HttpPost("getList")]
        public async Task<List<FrMScPlanConvertInfo>> GetPlanConvertList(FrMScPlanConvertInfo planConvertInfo)
        {
            var result = await _planConvertService.GetPlanConvertList(planConvertInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getListById")]
        public async Task<FrMScPlanConvertInfo> GetPlanConvertById(FrMScPlanConvertInfo planConvertInfo)
        {
            return await _planConvertService.GetPlanConvertById(planConvertInfo);
        }

        // 情報追加
        [HttpPost("addPlanConvert")]
        public async Task<int> AddPlanConvert(FrMScPlanConvertInfo planConvertInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _planConvertService.AddPlanConvert(planConvertInfo);
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
        [HttpPut("updatePlanConvert")]
        public async Task<CommonEnum.DBUpdateResult> UpdatePlanConvert(FrMScPlanConvertInfo planConvertInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _planConvertService.UpdatePlanConvert(planConvertInfo, addFlag);
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
        [HttpPut("delPlanConvert")]
        public async Task<CommonEnum.DBUpdateResult> DelPlanConvert(FrMScPlanConvertInfo planConvertInfo)
        {
            try
            {
                int ret = await _planConvertService.DelPlanConvert(planConvertInfo);
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