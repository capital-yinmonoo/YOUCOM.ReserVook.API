using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信エージェント情報</summary>
    public class FrDScRcvAgentIfInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }
        ///<summary>ＳＣ受信エージェント情報ＳＥＱ</summary>
        public int ScRcvAgentSeq { get; set; }
        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }

        ///<summary>ＸＭＬポイント区分</summary>
        public string XPntDiv { get; set; }
        ///<summary>ＸＭＬポイント名称・補助金名称</summary>
        public string XPntNm { get; set; }
        ///<summary>ＸＭＬポイント割引金額・補助金額</summary>
        public int XPnts { get; set; }
        ///<summary>ＸＭＬ割引後の総額</summary>
        public int XTtlAccmDeclPnts { get; set; }
        ///<summary>ＸＭＬ割引後の総額に対する、消費税額</summary>
        public int XTtlAccmCnsmptTax { get; set; }
        ///<summary>ＸＭＬ宿泊者請求額</summary>
        public int XAmntClaimed { get; set; }
        ///<summary>ＸＭＬVIPコード</summary>
        public string XVipCd { get; set; }
        ///<summary>ＸＭＬ変更前予約番号</summary>
        public string XAgoRsvNum { get; set; }
        ///<summary>ＸＭＬ変更後予約番号</summary>
        public string XFrRsvNum { get; set; }
        ///<summary>ＸＭＬ当日予約</summary>
        public string XTodayReserve { get; set; }
        ///<summary>ＸＭＬ合計男性人数</summary>
        public int XTtlMaleCnt { get; set; }
        ///<summary>ＸＭＬ合計女性人数</summary>
        public int XTtlFemaleCnt { get; set; }
        ///<summary>ＸＭＬ事前決済区分</summary>
        public string XSettlementDiv { get; set; }
        ///<summary>ＸＭＬ事前決済に対するキャンセル料金</summary>
        public string XCancellationChg { get; set; }
        ///<summary>ＸＭＬ取消料補足説明事項</summary>
        public string XCancellationNotice { get; set; }

        ///<summary>予約番号</summary>
        public string ReservationNo { get; set; }
        ///<summary>出発日</summary>
        public string CheckoutDate { get; set; }
        ///<summary>更新回数</summary>
        public int UpdateCnt { get; set; }

        [NotMapped]
        public string PntDivDisplay { get; set; }


        ///<summary>
        /// プロパティ名と値を文字列として出力します
        ///</summary>
        public override string ToString()
        {
            return ""
                    + "CompanyNo : " + CompanyNo + "|"
                    + "ScCd : " + ScCd + "|"

                    + "ScRcvSeq : " + ScRcvSeq.ToString() + "|"
                    + "ScRcvAgentSeq : " + ScRcvAgentSeq.ToString() + "|"
                    + "XTravelAgncBkngNum : " + XTravelAgncBkngNum + "|"

                    + "XPntDiv : " + XPntDiv + "|"
                    + "XPntNm : " + XPntNm + "|"
                    + "XPnts : " + XPnts.ToString() + "|"
                    + "XTtlAccmDeclPnts : " + XTtlAccmDeclPnts.ToString() + "|"
                    + "XTtlAccmCnsmptTax : " + XTtlAccmCnsmptTax.ToString() + "|"
                    + "XAmntClaimed : " + XAmntClaimed.ToString() + "|"
                    + "XVipCd : " + XVipCd + "|"
                    + "XAgoRsvNum : " + XAgoRsvNum + "|"
                    + "XFrRsvNum : " + XFrRsvNum + "|"
                    + "XTodayReserve : " + XTodayReserve + "|"
                    + "XTtlMaleCnt : " + XTtlMaleCnt.ToString() + "|"
                    + "XTtlFemaleCnt : " + XTtlFemaleCnt.ToString() + "|"
                    + "XSettlementDiv : " + XSettlementDiv + "|"
                    + "XCancellationChg : " + XCancellationChg + "|"
                    + "XCancellationNotice : " + XCancellationNotice + "|"

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
