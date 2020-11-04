using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信リザプリ部屋料金情報</summary>
    public class FrDScRcvRpRmRtIfInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }
        ///<summary>ＳＣ受信リザプリ部屋情報ＳＥＱ</summary>
        public int ScRcvRpRmIfSeq { get; set; }
        ///<summary>ＳＣ受信リザプリ部屋料金情報ＳＥＱ</summary>
        public int ScRcvRpRmRtIfSeq { get; set; }
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

        ///<summary>ＸＭＬ1室あたり宿泊料金合計</summary>
        public int XTtlPerRmRt { get; set; }
        ///<summary>ＸＭＬ1室あたり宿泊料金消費税合計</summary>
        public int XTtlPerRmCnsmptTax { get; set; }
        ///<summary>ＸＭＬ1室あたり宿泊料金入湯税合計</summary>
        public int XTtlRmHotsprTax { get; set; }
        ///<summary>ＸＭＬ1室あたり宿泊料金サービス料合計</summary>
        public int XTtlPerRmSrvcFee { get; set; }

        ///<summary>ＸＭＬ大人(男)一人料金</summary>
        public int XPerMaleRt { get; set; }
        ///<summary>ＸＭＬ大人(女)一人料金</summary>
        public int XPerFemaleRt { get; set; }
        ///<summary>ＸＭＬ大人人員（男性）</summary>
        public int XRmRtPaxMaleCnt { get; set; }
        ///<summary>ＸＭＬ大人人員（女性）</summary>
        public int XRmRtPaxFemaleCnt { get; set; }
        ///<summary>ＸＭＬ子供A人数</summary>
        public int XRmRtChildA70Cnt { get; set; }
        ///<summary>ＸＭＬ子供B人数</summary>
        public int XRmRtChildB50Cnt { get; set; }
        ///<summary>ＸＭＬ子供C人数</summary>
        public int XRmRtChildC30Cnt { get; set; }
        ///<summary>ＸＭＬ子供D人数</summary>
        public int XRmRtChildDNoneCnt { get; set; }

        ///<summary>ＸＭＬ大人（男性）リクエスト</summary>
        public string XRmRtPaxMaleReq { get; set; }
        ///<summary>ＸＭＬ大人（女性）リクエスト</summary>
        public string XRmRtPaxFemaleReq { get; set; }
        ///<summary>ＸＭＬ子供Aリクエスト</summary>
        public string XRmRtChildA70Req { get; set; }
        ///<summary>ＸＭＬ子供Bリクエスト</summary>
        public string XRmRtChildB50Req { get; set; }
        ///<summary>ＸＭＬ子供Cリクエスト</summary>
        public string XRmRtChildC30Req { get; set; }
        ///<summary>ＸＭＬ子供Dリクエスト</summary>
        public string XRmRtChildDNoneReq { get; set; }

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
                    + "ScRcvRpRmIfSeq : " + ScRcvRpRmIfSeq.ToString() + "|"
                    + "ScRcvRpRmRtIfSeq : " + ScRcvRpRmRtIfSeq.ToString() + "|"
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
