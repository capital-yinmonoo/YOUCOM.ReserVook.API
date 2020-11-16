using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IRoomsService
    {
        #region 部屋マスタ関連
        Task<int> AddRoomInfo(MstRoomsInfo roomInfo);
        Task<int> UpdateRoomInfo(MstRoomsInfo roomInfo, bool addFlag);
        Task<int> DelRoomInfo(MstRoomsInfo roomInfo);

        Task<RoomInfoView> GetRoomInfoById(MstRoomsInfo roomInfo);
        Task<List<RoomInfoView>> GetList(MstRoomsInfo roomInfo);


        Task<int> DeleteRoomCheckAssign(MstRoomsInfo roomInfo);

        Task<int> DeleteRoomCheckCleaned(MstRoomsInfo roomInfo);

        // 部屋取得(他ページ用)
        Task<List<MstRoomsInfo>> GetRoomList(string companyNo);
        #endregion

        #region 部屋表示設定関連
        Task<CommonEnum.DBUpdateResult> UpdateRoomDispLocation(List<MstRoomsInfo> list);
        #endregion

        #region アサイン関連
        Task<List<List<RoomsAssignedInfo>>> GetDailyAssign(RoomsAssignCondition cond);

      

        Task<List<NotAssignInfo>> GetDailyNotAssignInfo(RoomsAssignCondition cond);

        Task<int> AssignRoom(List<TrnReserveAssignInfo> list);

        Task<CommonEnum.DBUpdateResult> AssignCancel(TrnReserveAssignInfo cond);

        Task<List<TrnReserveAssignInfo>> GetReserveNotAssignInfo(TrnReserveAssignInfo info);

        Task<TrnReserveAssignInfo> CheckRoomStateAndSearchAssignableRoom(TrnReserveAssignInfo info);

        #endregion

        #region チェックイン関連
        Task<int> CheckIn(TrnReserveAssignInfo cond);

        Task<CommonEnum.DBUpdateResult> CheckInCancel(TrnReserveAssignInfo cond);
        #endregion

        #region チェックアウト関連
        Task<int> CheckOut(TrnReserveAssignInfo cond);

        Task<int> CheckOutCancel(TrnReserveAssignInfo cond);
        #endregion

        #region 中抜け関連

        Task<int> Hollow(TrnReserveAssignInfo cond);

        Task<int> HollowCancel(TrnReserveAssignInfo cond);

        Task<int> HollowCheckin(TrnReserveAssignInfo cond);

        Task<bool> HollowIsCheckin(TrnReserveAssignInfo cond);

        Task<int> HollowCheckinCancel(TrnReserveAssignInfo cond);

        Task<bool> HollowIsCheckinCancel(TrnReserveAssignInfo cond);
        
        #endregion

        #region 清掃関連
        Task<CommonEnum.DBUpdateResult> CleaningRoom(TrnReserveAssignInfo info);
        Task<CommonEnum.DBUpdateResult> CleaningStayRoom(TrnReserveAssignInfo info);
        #endregion

        #region ルームチェンジ
        Task<RoomChangeResult> RoomChange(RoomsAssignedInfo baseRoom, RoomsAssignedInfo TargetRoom);
        #endregion

        #region 連泊状況関連
        Task<List<BookingsInfo>> GetBookingsList(BookingsCondition cond);
        #endregion

        #region クリーニングリスト
        Task<List<CleaningInfo>> GetCleaningList(CleaningCondition cond);
        #endregion

        #region クリーニングリスト 客室状態更新
        Task<int> UpdateRoomStatus(TrnReserveAssignInfo roomInfo);
        #endregion

        #region 部屋割詳細
        Task<ReserveModel> GetAssignListByReserveNo(TrnReserveAssignInfo cond);

        Task<int> UpdateRoomDetails(UpdateRoomDetails info);

        Task<RequestDataInfo> GetRequestData(RequestDataCondition cond);

        #endregion

        #region
        Task<RequestDataInfo> GetRequestData(RequestDataCondition cond);
        #endregion


    }
}
