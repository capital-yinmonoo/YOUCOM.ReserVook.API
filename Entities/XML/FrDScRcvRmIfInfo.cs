namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信部屋情報</summary>
    public class FrDScRcvRmIfInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }
        ///<summary>ＳＣ受信部屋情報ＳＥＱ</summary>
        public int ScRcvRmIfSeq { get; set; }
        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }

        ///<summary>ＸＭＬ部屋タイプコード</summary>
        public string XRmTypeCd { get; set; }
        ///<summary>ＸＭＬ部屋タイプ名</summary>
        public string XRmTypeNm { get; set; }
        ///<summary>ＸＭＬ部屋カテゴリー</summary>
        public string XRmCategory { get; set; }
        ///<summary>ＸＭＬ部屋眺望</summary>
        public string XViewType { get; set; }
        ///<summary>ＸＭＬ喫煙/禁煙</summary>
        public string XSmokingOrNonSmoking { get; set; }

        ///<summary>ＸＭＬ1室利用人数</summary>
        public int XPerRmPaxCnt { get; set; }
        ///<summary>ＸＭＬ大人人員(男性)</summary>
        public int XRmPaxMaleCnt { get; set; }
        ///<summary>ＸＭＬ大人人員(女性)</summary>
        public int XRmPaxFemaleCnt { get; set; }
        ///<summary>ＸＭＬ子供A人数</summary>
        public int XRmChildA70Cnt { get; set; }
        ///<summary>ＸＭＬ子供B人数</summary>
        public int XRmChildB50Cnt { get; set; }
        ///<summary>ＸＭＬ子供C人数</summary>
        public int XRmChildC30Cnt { get; set; }
        ///<summary>ＸＭＬ子供D人数</summary>
        public int XRmChildDNoneCnt { get; set; }
        ///<summary>ＸＭＬ子供Ｅ人数</summary>
        public int XRmChildENoneCnt { get; set; }
        ///<summary>ＸＭＬ子供Ｆ人数</summary>
        public int XRmChildFNoneCnt { get; set; }
        ///<summary>ＸＭＬ子供その他人数</summary>
        public int XRmChildOtherCnt { get; set; }

        ///<summary>ＸＭＬ部屋毎予約ステイタス</summary>
        public string XRmByRmStatus { get; set; }
        ///<summary>ＸＭＬその他設備</summary>
        public string XFacilities { get; set; }
        ///<summary>ＸＭＬ部屋割り後客室名/番号</summary>
        public string XAssignedRmNum { get; set; }
        ///<summary>ＸＭＬ客室に対する特別リクエスト</summary>
        public string XRmSpecialReq { get; set; }

        ///<summary>ＳＣ受信部屋料金情報カウンタ</summary>
        public int ScRcvRmRtIfCntr { get; set; }
        ///<summary>ＳＣ受信部屋宿泊者情報カウンタ</summary>
        public int ScRcvRmGstIfCntr { get; set; }

        ///<summary>予約番号</summary>
        public string ReservationNo { get; set; }
        ///<summary>出発日</summary>
        public string CheckoutDate { get; set; }
        ///<summary>更新回数</summary>
        public int UpdateCnt { get; set; }

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
