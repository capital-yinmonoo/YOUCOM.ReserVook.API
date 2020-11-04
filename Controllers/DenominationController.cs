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
    public class DenominationController : ControllerBase
    {


        private IDenominationService _denominationService;

        public DenominationController(IDenominationService denominationService)
        {
            _denominationService = denominationService;

        }

        // 情報取得(画面表示,削除用)
        [HttpPost("getlist")]
        public async Task<List<MstDenominationInfo>> GetDenominationList(MstDenominationInfo denominationInfo)
        {
            var result = await _denominationService.GetDenominationList(denominationInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getListById")]
        public async Task<MstDenominationInfo> GetDenominationById(MstDenominationInfo denominationInfo)
        {
            return await _denominationService.GetDenominationById(denominationInfo);
        }

        // 情報追加
        [HttpPost("addDenomination")]
        public async Task<int> AddDenomination(MstDenominationInfo denominationInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _denominationService.AddDenomination(denominationInfo);
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
        [HttpPut("updateDenomination")]
        public async Task<CommonEnum.DBUpdateResult> UpdateDenomination(MstDenominationInfo denominationInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _denominationService.UpdateDenomination(denominationInfo, addFlag);
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
        [HttpPut("delDenomination")]
        public async Task<CommonEnum.DBUpdateResult> DelDenomination(MstDenominationInfo denominationInfo)
        {
            try
            {
                int ret = await _denominationService.DelDenomination(denominationInfo);
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

        // 金種取得(別ページ用)
        [HttpGet("getDenominationList")]
        public async Task<List<MstDenominationInfo>> GetDenominationList(string companyNo)
        {
            var model = await _denominationService.GetDenominationList(companyNo);
            return model;

        }

        // 削除チェック
        [HttpPut("deleteDenominationCheck")]
        public async Task<int> DeleteDenominationCheck(MstDenominationInfo denominationInfo)
        {
            int result = await _denominationService.DeleteDenominationCheck(denominationInfo);
            return result;
        }
    }
}