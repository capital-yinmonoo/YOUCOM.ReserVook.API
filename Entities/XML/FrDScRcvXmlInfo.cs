namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信ＸＭＬデータ</summary>
    public class FrDScRcvXmlInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }
        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }

        ///<summary>ＳＣ受信ＸＭＬ</summary>
        public string ScRcvXml { get; set; }
        ///<summary>受信ＸＭＬファイル名</summary>
        public string ScRcvFileNm { get; set; }

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
