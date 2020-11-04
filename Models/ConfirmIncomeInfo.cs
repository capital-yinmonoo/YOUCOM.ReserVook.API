using System.ComponentModel;

namespace YOUCOM.ReserVook.API.Models
{
    public partial class ConfirmIncomeInfo
    {

        /// <summary>
        /// 予約番号
        /// </summary>
        public string ReserveNo { get; set; }

        /// <summary>
        /// 到着日
        /// </summary>
        public string ArrivalDate { get; set; }

        /// <summary>
        /// 出発日
        /// </summary>
        public string DepartureDate { get; set; }

        /// <summary>
        /// 泊数
        /// </summary>
        public string StayDays { get; set; }

        /// <summary>
        /// 顧客番号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 利用者名
        /// </summary>
        public string GuestName { get; set; }

        /// <summary>
        /// 前日迄売上
        /// </summary>
        public string DayBeforeSales { get; set; }

        /// <summary>
        /// 当日売上
        /// </summary>
        public string TodaySales { get; set; }

        /// <summary>
        /// 前日迄入金
        /// </summary>
        public string DayBeforeDeposit { get; set; }

        /// <summary>
        /// (当日入金)金種コード
        /// </summary>
        public string DenominationCode { get; set; }

        /// <summary>
        /// (当日入金)金種
        /// </summary>
        public string DenominationName { get; set; }

        /// <summary>
        /// (当日入金)金額
        /// </summary>
        public string AmountPrice { get; set; }

        /// <summary>
        /// 残高
        /// </summary>
        public string Balance { get; set; }

        /// <summary>
        /// 合計行判定
        /// </summary>
        public bool totalflag { get; set; }

        public void Init(bool info = false)
        {

            ReserveNo = "";
            ArrivalDate = "";
            DepartureDate = "";
            StayDays = "";
            CustomerNo = "";
            GuestName = "";
            DayBeforeSales = "";
            TodaySales = "";
            DayBeforeDeposit = "";
            DenominationCode = "";
            DenominationName = "";
            AmountPrice = "";
            Balance = "";
            totalflag = info;

        }
    }
}
