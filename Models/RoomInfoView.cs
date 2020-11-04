using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    public class RoomInfoView : MstRoomsInfo
    {
        /// <summary>部屋タイプ名称</summary>
        public string RoomTypeName { get; set; }
        /// <summary>フロア名称</summary>
        public string FloorName { get; set; }
        /// <summary>禁煙/喫煙名称</summary>
        public string SmokingDivisionName { get; set; }
    }
}
