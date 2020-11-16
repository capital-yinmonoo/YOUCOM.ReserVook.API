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
    public class RoomsController : ControllerBase
    {
        private readonly ILogger<RoomsController> _logger;

        private IRoomsService _roomsService;
        private IReserveService _reserveService;

        public RoomsController(ILogger<RoomsController> logger, IRoomsService roomsService, IReserveService reserveService)
        {
            _logger = logger;

            _roomsService = roomsService;
            _reserveService = reserveService;
        }

        #region 部屋マスタ関連
        /// <summary>
        /// 部屋マスタ 追加
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        [HttpPost("addroominfo")]
        public async Task<int> AddRoomInfo(MstRoomsInfo roomInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _roomsService.AddRoomInfo(roomInfo);
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

        /// <summary>
        /// 部屋マスタ 更新
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        [HttpPut("updateroominfo")]
        public async Task<CommonEnum.DBUpdateResult> UpdateRoomInfo(MstRoomsInfo roomInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _roomsService.UpdateRoomInfo(roomInfo, addFlag);
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

        /// <summary>
        /// 部屋マスタ 削除
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        [HttpPut("delroomInfo")]
        public async Task<CommonEnum.DBUpdateResult> DelRoomInfo(MstRoomsInfo roomInfo)
        {
            try
            {
                int ret = await _roomsService.DelRoomInfo(roomInfo);
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


        /// <summary>
        /// 部屋マスタ 取得
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        [HttpPost("getroominfobyid")]
        public async Task<RoomInfoView> GetRoomInfoById(MstRoomsInfo roomInfo)
        {
            return await _roomsService.GetRoomInfoById(roomInfo);
        }

        /// <summary>
        /// 部屋マスタ 一覧取得
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        [HttpPost("getlist")]
        public async Task<List<RoomInfoView>> GetList(MstRoomsInfo roomInfo)
        {
            var result = await _roomsService.GetList(roomInfo);
            return result;
        }


        /// <summary>
        /// 部屋マスタ 削除部屋のアサインされているかをチェック
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        [HttpPut("deleteroomcheckassign")]
        public async Task<int> DeleteRoomCheckAssign(MstRoomsInfo roomInfo)
        {
            int result = await _roomsService.DeleteRoomCheckAssign(roomInfo);
            return result;
        }

        /// <summary>
        /// 部屋マスタ 削除部屋が清掃完了済かをチェック
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        [HttpPost("deleteroomcheckcleaned")]
        public async Task<int> DeleteRoomCheckCleaned(MstRoomsInfo roomInfo)
        {
            int result = await _roomsService.DeleteRoomCheckCleaned(roomInfo);
            return result;
        }

        // 部屋取得(他ページ用)
        [HttpGet("getRoomList")]
        public async Task<List<MstRoomsInfo>> GetRoomList(string companyNo)
        {
            var model = await _roomsService.GetRoomList(companyNo);
            return model;

        }
        #endregion

        #region 部屋表示設定関連
        /// <summary>
        /// 部屋の画面位置更新
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("updateRoomDispLocation")]
        public async Task<CommonEnum.DBUpdateResult> UpdateRoomDispLocation(List<MstRoomsInfo> list)
        {
            return await _roomsService.UpdateRoomDispLocation(list);
        }
        #endregion

        #region アサイン関連
        /// <summary>
        /// 日別にアサイン情報を取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getDailyAssignInfo")]
        public async Task<List<List<RoomsAssignedInfo>>> GetDailyAssignInfo(RoomsAssignCondition cond)
        {
            return await _roomsService.GetDailyAssign(cond);
        }
              

        /// <summary>
        /// 日別に未アサインリスト取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getDailyNotAssignInfo")]
        public async Task<List<NotAssignInfo>> GetDailyNotAssignInfo(RoomsAssignCondition cond)
        {
            return await _roomsService.GetDailyNotAssignInfo(cond);
        }

        /// <summary>
        /// 予約番号の指定日以降の未アサインを取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getReserveNotAssignInfo")]
        public async Task<List<TrnReserveAssignInfo>> GetReserveNotAssignInfo(TrnReserveAssignInfo cond)
        {
            return await _roomsService.GetReserveNotAssignInfo(cond);
        }

        /// <summary>
        /// アサイン
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("assignRoom")]
        public async Task<int> AssignRoom(List<TrnReserveAssignInfo> list)
        {
            return await _roomsService.AssignRoom(list);
        }

        /// <summary>
        /// アサイン解除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("assignCancel")]
        public async Task<CommonEnum.DBUpdateResult> AssignCancel(TrnReserveAssignInfo cond)
        {
            return await _roomsService.AssignCancel(cond);
        }

        #endregion

        #region チェックイン関連
        /// <summary>
        /// チェックイン
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("checkin")]
        public async Task<int> CheckIn(TrnReserveAssignInfo cond)
        {
            return await _roomsService.CheckIn(cond);
        }

        /// <summary>
        /// チェックイン取消
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("checkInCancel")]
        public async Task<CommonEnum.DBUpdateResult> CheckInCancel(TrnReserveAssignInfo cond)
        {
            return await _roomsService.CheckInCancel(cond);
        }
        #endregion

        #region チェックアウト関連
        /// <summary>
        /// チェックアウト
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("checkOut")]
        public async Task<int> CheckOut(TrnReserveAssignInfo cond)
        {
            return await _roomsService.CheckOut(cond);
        }

        /// <summary>
        /// チェックアウト取消
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("checkOutCancel")]
        public async Task<int> CheckOutCancel(TrnReserveAssignInfo cond)
        {
            return await _roomsService.CheckOutCancel(cond);
        }

        #endregion

        #region 中抜け関連

        /// <summary>
        /// 中抜け
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("hollow")]
        public async Task<int> Hollow(TrnReserveAssignInfo cond)
        {
            return await _roomsService.Hollow(cond);
        }

        /// <summary>
        /// 中抜け取消
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("hollowCancel")]
        public async Task<int> HollowCancel(TrnReserveAssignInfo cond)
        {
            return await _roomsService.HollowCancel(cond);
        }

        /// <summary>
        /// 中抜けチェックイン
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("hollowCheckin")]
        public async Task<int> HollowCheckin(TrnReserveAssignInfo cond)
        {
            return await _roomsService.HollowCheckin(cond);
        }

        /// <summary>
        /// 中抜けチェックイン可否判定
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("HollowIsCheckin")]
        public async Task<bool> HollowIsCheckin(TrnReserveAssignInfo cond)
        {
            return await _roomsService.HollowIsCheckin(cond);
        }

        /// <summary>
        /// 中抜けチェックインキャンセル
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("hollowCheckinCancel")]
        public async Task<int> HollowCheckinCancel(TrnReserveAssignInfo cond) {
            return await _roomsService.HollowCheckinCancel(cond);
        }

        /// <summary>
        /// 中抜けチェックインキャンセル可否判定
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("HollowIsCheckinCancel")]
        public async Task<bool> HollowIsCheckinCancel(TrnReserveAssignInfo cond) {
            return await _roomsService.HollowIsCheckinCancel(cond);
        }
        #endregion


        #region 清掃関連    
        /// <summary>
        /// 清掃完了
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("cleaningRoom")]
        public async Task<CommonEnum.DBUpdateResult> CleaningRoom(TrnReserveAssignInfo cond)
        {
            return await _roomsService.CleaningRoom(cond);
        }

        /// <summary>
        /// 清掃完了(滞在部屋)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("cleaningStayRoom")]
        public async Task<CommonEnum.DBUpdateResult> CleaningStayRoom(TrnReserveAssignInfo cond) {
            return await _roomsService.CleaningStayRoom(cond);
        }
        #endregion

        #region ルームチェンジ
        /// <summary>
        /// ルームチェンジ
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("roomChange")]
        public async Task<RoomChangeResult> RoomChange(List<RoomsAssignedInfo> cond)
        {
            //cond[0]:移動元, cond[1]:移動先
            if (cond == null || cond.Count() != 2)
            {
                var ret = new RoomChangeResult();
                ret.Result = CommonEnum.DBUpdateResult.Error;
                return ret;
            }

            return await _roomsService.RoomChange(cond[0], cond[1]);
        }
        #endregion

        #region 部屋割詳細関連
        /// <summary>
        /// 予約番号のアサインを取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getAssignListByReserveNo")]
        public async Task<ReserveModel> GetAssignListByReserveNo(TrnReserveAssignInfo cond)
        {
            return await _roomsService.GetAssignListByReserveNo(cond);
        }

        /// <summary>
        /// アサイン情報/氏名ファイルを更新
        /// </summary>
        /// <param name="info">更新データ</param>
        /// <returns></returns>
        [HttpPost("updateRoomDetails")]
        public async Task<int> UpdateRoomDetails(UpdateRoomDetails info)
        {
            return await _roomsService.UpdateRoomDetails(info);
        }

        #endregion

        /// <summary>
        /// get guest information 
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getRequestDataInfo")]
        public async Task<RequestDataInfo> GetRequestDataInfo(RequestDataCondition cond)
        {
            return await _roomsService.GetRequestData(cond);
        }

    }
}