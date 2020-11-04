using System.Collections.Generic;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Entities
{
    // HACK: フロント側とAPI側の変数名は必ず合わせること。名前が異なると値がとれないため。

    public partial class ReserveInfo
    {
        public ReserveInfo()
        {
            RoomTypeInfoList = new List<TrnReserveRoomtypeInfo>();
            SalesDetailsInfoList = new List<TrnSalesDetailsInfo>();
            DepositInfoList = new List<TrnDepositDetailsInfo>();
            RemarksInfoList = new List<TrnReserveNoteInfo>();
        }

        public TrnReserveBasicInfo ReserveBasicInfo { get; set; }
        public TrnNameFileInfo NameFileInfo { get; set; }

        public List<TrnNameFileInfo> NameFileInfo_Room { get; set; }

        public List<TrnReserveRoomtypeInfo> RoomTypeInfoList { get; set; }
        public List<TrnReserveAssignInfo> AssignInfoList { get; set; }
        public List<TrnSalesDetailsInfo> SalesDetailsInfoList { get; set; }
        public List<TrnDepositDetailsInfo> DepositInfoList { get; set; }
        public List<TrnReserveNoteInfo> RemarksInfoList { get; set; }
    }

    /// <summary>
    /// 予約登録画面のModelまとめ
    /// </summary>
    public partial class ReserveModel : BaseInfo
    {

        public string ReserveNo { get; set; }

        public StayInfo stayInfo { get; set; }
        public GuestInfo guestInfo { get; set; }
        public AgentInfo agentInfo { get; set; }
        public IList<RoomTypeInfo> roomTypeInfo { get; set; }
        public IList<SalesDetailsInfo> itemInfo { get; set; }

        //public IList<SalesDetailsInfo> otherItemInfo { get; set; }
        public IList<DepositInfo> depositInfo { get; set; }
        public IList<RemarksInfo> remarksInfo { get; set; }

        public List<TrnReserveFacilityInfo> trnFacilityInfo { get; set; }

        public List<TrnReserveAssignInfo> assignList { get; set; }
        public List<int> adjustmentedBillNoCheckList { get; set; }

        public TrnReserveAssignInfo assignInfo { get; set; }

        // 連泊画面から登録時 アサイン用部屋番号
        public string assignRoomNo { get; set; }

        // 部屋割詳細情報 上書き確認用
        public bool hasRoomsNameFile  { get; set; }

        public string XTravelAgncBkngNum { get; set; }
        public string ScCd { get; set; }
        public int XTravelAgncBkngSeq { get; set; }
    }

    public partial class StayInfo : BaseInfo
    {
        public string ReserveNo { get; set; }

        public string ArrivalDate { get; set; }
        public int StayDays { get; set; }
        public string DepartureDate { get; set; }
        public string ReserveDate { get; set; }
        public int MemberMale { get; set; }
        public int MemberFemale { get; set; }
        public int MemberChildA { get; set; }
        public int MemberChildB { get; set; }
        public int MemberChildC { get; set; }

        public string AdjustmentFlag { get; set; }

        public string ReserveStateDivision { get; set; }
    }


    public partial class GuestInfo : BaseInfo
    {
        public int NameSeq { get; set; }
        public string RoomNo { get; set; }
        public string Phone { get; set; }
        public string Cellphone { get; set; }
        public string GuestName { get; set; }
        public string GuestNameKana { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string CustomerNo { get; set; }

        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }

        /// <summary>備考1</summary>
        public string Remarks1 { get; set; }

        /// <summary>備考2</summary>
        public string Remarks2 { get; set; }

        /// <summary>備考3</summary>
        public string Remarks3 { get; set; }

        /// <summary>備考4</summary>
        public string Remarks4 { get; set; }

        /// <summary>備考5</summary>
        public string Remarks5 { get; set; }

        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>ルートSEQ</summary>
        public int RouteSEQ { get; set; }

        /// <summary>氏名ファイル 上書きフラグ</summary>
        public bool OverwriteFlag { get; set; }
    }

    public partial class AgentInfo
    {
        public string AgentCode { get; set; }
        public string AgentRemarks { get; set; }
    }

    //public partial class RoomTypeInfoModel
    //{
    //    public string RoomType { get; set; }

    //    //public int RoomCount { get; set; }
    //}

    public partial class SalesDetailsInfo : BaseInfo
    {
        public string ReserveNo { get; set; }

        public int DetailsSeq { get; set; }

        public string ItemDivision { get; set; }

        public string MealDivision { get; set; }

        public string UseDate { get; set; }

        public string ItemCode { get; set; }

        public string PrintName { get; set; }
        public decimal UnitPrice { get; set; }
        public int ItemNumberM { get; set; }
        public int ItemNumberF { get; set; }
        public int ItemNumberC { get; set; }
        public decimal AmountPrice { get; set; }
        public decimal InsideTaxPrice { get; set; }
        public decimal InsideServicePrice { get; set; }
        public decimal OutsideServicePrice { get; set; }
        public string TaxDivision { get; set; }
        public string TaxRateDivision { get; set; }
        public string ServiceDivision { get; set; }
        public string SetItemDivision { get; set; }
        public int SetItemSeq { get; set; }
        public decimal TaxRate { get; set; }
        public int BillSeparateSeq { get; set; }
        public string BillNo { get; set; }
        public string AdjustmentFlag { get; set; }

        // 更新用フラグ

        /// <summary>追加フラグ</summary>
        public bool AddFlag { get; set; }

        /// <summary>更新フラグ</summary>
        public bool UpdateFlag { get; set; }

        /// <summary>削除フラグ</summary>
        public bool DeleteFlag { get; set; }
    }


    public partial class DepositInfo : BaseInfo
    {
        public int DetailsSeq { get; set; }

        public string DepositDate { get; set; }

        public string DenominationCode { get; set; }

        public string PrintName { get; set; }

        public decimal Price { get; set; }

        public string BillingRemarks { get; set; }

        public int BillSeparateSeq { get; set; }

        public string BillNo { get; set; }

        public string AdjustmentFlag { get; set; }

        // 更新用フラグ

        /// <summary>追加フラグ</summary>
        public bool AddFlag { get; set; }

        /// <summary>更新フラグ</summary>
        public bool UpdateFlag { get; set; }

        /// <summary>削除フラグ</summary>
        public bool DeleteFlag { get; set; }
    }


    public partial class RemarksInfo : BaseInfo
    {
        public int NoteSeq { get; set; }

        public string Remarks { get; set; }

        // 更新用フラグ

        /// <summary>追加フラグ</summary>
        public bool AddFlag { get; set; }

        /// <summary>更新フラグ</summary>
        public bool UpdateFlag { get; set; }

        /// <summary>削除フラグ</summary>
        public bool DeleteFlag { get; set; }
    }


    // 精算時の更新情報
    public partial class AdjustmentUpdateInfo {

        /// <summary>精算前に予約情報を更新する</summary>
        public ReserveModel reserve { get; set; }

        /// <summary>精算</summary>
        public SalesDetailsInfo adjustment { get; set; }

    }

    // アサイン戻り値
    public partial class ResultInfo
    {
        /// <summary>予約番号</summary>
        public string reserveNo { get; set; }

        /// <summary>予約戻り値</summary>
        public int reserveResult { get; set; }

        /// <summary>アサイン戻り値</summary>
        public int assignResult { get; set; }
    }


}
