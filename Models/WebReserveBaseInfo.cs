using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Models
{
    ///<summary>ＳＣ基本情報</summary>
    public class WebReserveBaseInfo
    {
        ///<summary>会社番号</summary>
        public string CompanyNo { get; set; }
        ///<summary>ＳＣコード</summary>
        public string ScCd { get; set; }
        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }

        ///<summary>ＸＭＬデータ種別</summary>
        public string XDataClsfic { get; set; }
        ///<summary>ＸＭＬデータＩＤ</summary>
        public string XDataId { get; set; }

        ///<summary>ＸＭＬ旅行会社コード</summary>
        public string XSalesOfcCompanyCd { get; set; }
        ///<summary>ＸＭＬ旅行会社名</summary>
        public string XSalesOfcCompanyNm { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所名</summary>
        public string XSalesOfcNm { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所コード</summary>
        public string XSalesOfcCd { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所担当者名</summary>
        public string XSalesOfcPrsnInChg { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所担当者Email</summary>
        public string XSalesOfcEmail { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所電話番号</summary>
        public string XSalesOfcPhnNum { get; set; }

        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }
        ///<summary>ＸＭＬ旅行会社予約受付日</summary>
        public string XTravelAgncBkngDate { get; set; }
        ///<summary>ＸＭＬ旅行会社受付時間</summary>
        public string XTravelAgncBkngTime { get; set; }
        ///<summary>ＸＭＬ通知番号</summary>
        public string XTravelAgncReportNum { get; set; }

        ///<summary>ＸＭＬ団体名または代表者氏名(半角)</summary>
        public string XGstOrGpNmSnglBt { get; set; }
        ///<summary>ＸＭＬ団体名または代表者氏名よみがな(全角)</summary>
        public string XGstOrGpNmDoubleBt { get; set; }
        ///<summary>ＸＭＬ団体名または代表者氏名 漢字</summary>
        public string XGstOrGpNmKanjiNm { get; set; }

        ///<summary>ＸＭＬチェックイン日</summary>
        public string XCheckInDate { get; set; }
        ///<summary>ＸＭＬチェックイン時間</summary>
        public string XCheckInTime { get; set; }
        ///<summary>ＸＭＬチェックアウト日</summary>
        public string XCheckOutDate { get; set; }
        ///<summary>ＸＭＬチェックアウト時間</summary>
        public string XCheckOutTime { get; set; }
        ///<summary>ＸＭＬ宿泊日数</summary>
        public int XNights { get; set; }

        ///<summary>ＸＭＬ利用客室合計数</summary>
        public int XTtlRmCnt { get; set; }
        ///<summary>ＸＭＬお客様総合計人数</summary>
        public int XGrandTtlPaxCnt { get; set; }

        ///<summary>ＸＭＬ企画(パッケージ)名</summary>
        public string XPackagePlanNm { get; set; }
        ///<summary>ＸＭＬ企画(パッケージ)コード</summary>
        public string XPackagePlanCd { get; set; }

        ///<summary>予約番号</summary>
        public string ReservationNo { get; set; }
        ///<summary>ＳＣ処理済みコード</summary>
        public string ScProcessedCd { get; set; }
        ///<summary>ＳＣ処理済みメッセージ</summary>
        public string ScProcessedMessage { get; set; }

        ///<summary>更新日時 : YYYYMMDD hhmmss</summary>
        public string UpdateDatetime { get; set; }

        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string TypeColor { get; set; }
        [NotMapped]
        public int NewCount { get; set; }
        [NotMapped]
        public int CancelCount { get; set; }
    }
}
