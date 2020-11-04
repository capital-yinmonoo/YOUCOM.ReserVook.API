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
    public class StateController : ControllerBase
    {


        private IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;

        }

        // 情報取得(画面表示,削除用)
        [HttpPost("getList")]
        public async Task<List<MstStateInfo>> GetStateList(MstStateInfo lostStateInfo)
        {
            var result = await _stateService.GetStateList(lostStateInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getListById")]
        public async Task<MstStateInfo> GetStateById(MstStateInfo lostStateInfo)
        {
            return await _stateService.GetStateById(lostStateInfo);
        }

        // 情報追加
        [HttpPost("addLostPlace")]
        public async Task<int> AddState(MstStateInfo lostStateInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _stateService.AddState(lostStateInfo);
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
        [HttpPut("updateLostPlace")]
        public async Task<CommonEnum.DBUpdateResult> UpdateState(MstStateInfo lostStateInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _stateService.UpdateState(lostStateInfo, addFlag);
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
        [HttpPut("delLostPlace")]
        public async Task<CommonEnum.DBUpdateResult> DelState(MstStateInfo lostStateInfo)
        {
            try
            {
                int ret = await _stateService.DelState(lostStateInfo);
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

        // 削除チェック
        [HttpPut("deleteLostPlaceCheck")]
        public async Task<int> DeleteStateCheck(MstStateInfo lostStateInfo)
        {
            int result = await _stateService.DeleteStateCheck(lostStateInfo);
            return result;
        }

        // 忘れ物状態取得(他マスタ用)
        [HttpGet("getStateList")]
        public async Task<List<MstStateInfo>> GetState(string companyNo)
        {
            var model = await _stateService.GetState(companyNo);
            return model;

        }
    }
}