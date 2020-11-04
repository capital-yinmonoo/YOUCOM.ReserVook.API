using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IRoomTypeConvertService
    {

        // 情報の取得(画面表示用)
        Task<List<FrMScRmtypeConvertInfo>> GetRoomTypeConvertList(FrMScRmtypeConvertInfo roomTypeConvertInfo);

        // 情報取得(編集,削除用)
        Task<FrMScRmtypeConvertInfo> GetRoomTypeConvertById(FrMScRmtypeConvertInfo roomTypeConvertInfo);

        // 情報追加
        Task<int> AddRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo);

        // 情報更新
        Task<int> UpdateRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo, bool addFlag);

        // 情報削除
        Task<int> DelRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo);
    }
}
