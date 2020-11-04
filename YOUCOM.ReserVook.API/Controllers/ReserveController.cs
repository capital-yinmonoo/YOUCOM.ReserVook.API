using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers
{
    //     [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReserveController : ControllerBase
    {
        private readonly ILogger<ReserveController> _logger;
        private IReserveService _reserveService;


        public ReserveController(ILogger<ReserveController> logger, IReserveService reserveService)
        {
            _logger = logger;

            _reserveService = reserveService;

        }

        // データ取得
        [HttpPost("getReserveInfoByPK")]
        public async Task<ReserveModel> GetReserveInfoByPK(ReserveModel cond)
        {
            return await _reserveService.GetReserveInfoByPK(cond);
        }

        // 登録
        [HttpPost("insertInfo")]
        public async Task<ResultInfo> InsertInfo(ReserveModel reserveInfo)
        {
            // 予約情報登録
            var result = await _reserveService.InsertReserveInfo(reserveInfo);
            return result;
        }

        // 更新
        [HttpPost("updateInfo")]
        public async Task<ResultInfo> UpdateInfo(ReserveModel reserveInfo)
        {
            // 予約情報 更新
            var result = await _reserveService.UpdateReserveInfo(reserveInfo);
            return result;

        }

        [HttpPost("updateInfo_ReserveCancel")]
        public async Task<int> UpdateInfo_ReserveCancel(StayInfo cond)
        {
            // 予約取消
            var result = await _reserveService.UpdatetReserveInfo_ReserveCancel(cond);
            return result;
        }

        [HttpPost("updateInfo_Adjustment")]
        public async Task<int> UpdateInfo_Adjustment(AdjustmentUpdateInfo cond)
        {
            // 予約情報 更新
            var resultInfo = await UpdateInfo(cond.reserve);
            if (resultInfo.reserveResult != (int)CommonEnum.DBUpdateResult.Success) return resultInfo.reserveResult;

            // 精算/精算取消
            var result = await _reserveService.UpdatetSalesDetailsInfo_AdjustmentFlag(cond.adjustment);
            return result;

        }

        #region -- マスタ類取得 --
        [HttpPost("getMasterRoomTypeList")]
        public async Task<List<MstCodeNameInfo>> GetMasterRoomTypeList(BaseInfo cond)
        {
            var result = await _reserveService.GetMasterRoomTypeList(cond);
            return result;
        }

        [HttpPost("getMasterAgentList")]
        public async Task<List<MstAgentInfo>> GetMasterAgentList(BaseInfo cond)
        {
            var result = await _reserveService.GetMasterAgentList(cond);
            return result;
        }


        [HttpPost("getMasterItemList_Stay")]
        public async Task<List<MstItemInfo>> GetMasterItemList_StayItem(BaseInfo cond)
        {
            var result = await _reserveService.GetMasterItemList_StayItem(cond);
            return result;
        }

        [HttpPost("getMasterItemList_Other")]
        public async Task<List<MstItemInfo>> GetMasterItemList_OtherItem(BaseInfo cond)
        {
            var result = await _reserveService.GetMasterItemList_OtherItem(cond);
            return result;
        }

        [HttpPost("getMasterItemList_Set")]
        public async Task<List<MstItemInfo>> GetMasterItemList_SetItem(BaseInfo cond)
        {
            var result = await _reserveService.GetMasterItemList_SetItem(cond);
            return result;
        }

        [HttpPost("getMasterSetItemList")]
        public async Task<List<MstSetItemInfo>> GetMasterSetItemList(BaseInfo cond)
        {
            var result = await _reserveService.GetMasterSetItemList(cond);
            return result;
        }

        [HttpPost("getMasterDenominationCodeList")]
        public async Task<List<MstDenominationInfo>> GetMasterDenominationCodeList(BaseInfo cond)
        {
            var result = await _reserveService.GetMasterDenominationCodeList(cond);
            return result;
        }
        #endregion


    }
}