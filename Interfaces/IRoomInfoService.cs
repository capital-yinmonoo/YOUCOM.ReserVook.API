using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IRoomInfoService
    {

        // 情報取得
        Task<MstRoomsInfo> GetRoomInfo(MstRoomsInfo roomInfo);

        // 更新
        Task<int> UpdateRoomInfo(MstRoomsInfo roomInfo);
    }
}
