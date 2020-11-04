using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信部屋料金情報</summary>
    public class FrDScRcvRmRtIfInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }
        ///<summary>ＳＣ受信部屋情報ＳＥＱ</summary>
        public int ScRcvRmIfSeq { get; set; }
        ///<summary>ＳＣ受信部屋料金情報ＳＥＱ</summary>
        public int ScRcvRmRtIfSeq { get; set; }
        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }

        ///<summary>ＸＭＬ利用年月日</summary>
        public string XRmDate { get; set; }
        ///<summary>ＸＭＬ大人一人料金</summary>
        public int XPerPaxRt { get; set; }
        ///<summary>ＸＭＬ子供A一人料金</summary>
        public int XPerChildA70Rt { get; set; }
        ///<summary>ＸＭＬ子供B一人料金</summary>
        public int XPerChildB50Rt { get; set; }
        ///<summary>ＸＭＬ子供C一人料金</summary>
        public int XPerChildC30Rt { get; set; }
        ///<summary>ＸＭＬ子供D一人料金</summary>
        public int XPerChildDRt { get; set; }
        ///<summary>ＸＭＬ子供Ｅ一人料金</summary>
        public int XPerChildERt { get; set; }
        ///<summary>ＸＭＬ子供Ｆ一人料金</summary>
        public int XPerChildFRt { get; set; }
        ///<summary>ＸＭＬ子供その他一人料金</summary>
        public int XPerChildOtherRt { get; set; }

        ///<summary>ＸＭＬ1室あたり宿泊料金合計</summary>
        public int XTtlPerRmRt { get; set; }
        ///<summary>ＸＭＬ1室あたり宿泊料金消費税合計</summary>
        public int XTtlPerRmCnsmptTax { get; set; }
        ///<summary>ＸＭＬ1室あたり宿泊料金入湯税合計</summary>
        public int XTtlRmHotsprTax { get; set; }
        ///<summary>ＸＭＬ1室あたり宿泊料金サービス料合計</summary>
        public int XTtlPerRmSrvcFee { get; set; }

        ///<summary>ＳＣ部屋別利用日客種一覧カウンタ</summary>
        public int ScRmDateGstListCntr { get; set; }

        ///<summary>予約番号</summary>
        public string ReservationNo { get; set; }
        ///<summary>出発日</summary>
        public string CheckoutDate { get; set; }
        ///<summary>更新回数</summary>
        public int UpdateCnt { get; set; }

        [NotMapped]
        public string XRmDateDisplayMD { get; set; }

        ///<summary>
        /// プロパティ名と値を文字列として出力します
        ///</summary>
        public override string ToString()
        {
            return ""
                    + "CompanyNo : " + CompanyNo + "|"
                    + "ScCd : " + ScCd + "|"

                    + "ScRcvSeq : " + ScRcvSeq.ToString() + "|"
                    + "ScRcvRmIfSeq : " + ScRcvRmIfSeq.ToString() + "|"
                    + "ScRcvRmRtIfSeq : " + ScRcvRmRtIfSeq.ToString() + "|"
                    + "XTravelAgncBkngNum : " + XTravelAgncBkngNum + "|"

                    + "ReservationNo : " + ReservationNo + "|"
                    + "CheckoutDate : " + CheckoutDate + "|"
                    + "UpdateCnt : " + UpdateCnt.ToString() + "|"

                    + "ProgramId : " + ProgramId + "|"
                    + "CreateClerkCd : " + CreateClerkCd + "|"
                    + "CreateMachineNo : " + CreateMachineNo + "|"
                    + "CreateMachine : " + CreateMachine + "|"
                    + "CreateDatetime : " + CreateDatetime + "|"
                    + "UpdateClerkCd : " + UpdateClerkCd + "|"
                    + "UpdateMachineNo : " + UpdateMachineNo + "|"
                    + "UpdateMachine : " + UpdateMachine + "|"
                    + "UpdateDatetime : " + UpdateDatetime + "|"
                    ;
        }
    }
}
