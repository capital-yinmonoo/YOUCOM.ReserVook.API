namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信リザプリ基本</summary>
    public class FrDScRcvRpBaseInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }
        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }

        ///<summary>ＸＭＬリザプリ電文</summary>
        public string XTelegramData { get; set; }
        ///<summary>ＸＭＬリザプリ電文２</summary>
        public string XTelegramData2 { get; set; }

        ///<summary>ＸＭＬ団体または代表者番号</summary>
        public string XPhnNum { get; set; }
        ///<summary>ＸＭＬ団体または代表者Email</summary>
        public string XEmail { get; set; }
        ///<summary>ＸＭＬ団体または代表者郵便番号</summary>
        public string XPostCd { get; set; }
        ///<summary>ＸＭＬ団体または代表者住所</summary>
        public string XAddress { get; set; }
        ///<summary>ＸＭＬ大人人数</summary>
        public int XTtlPaxManCnt { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所FAX番号</summary>
        public string XBranchFaxNum { get; set; }
        ///<summary>ＸＭＬTravelXMLバージョン</summary>
        public string XVer { get; set; }
        ///<summary>ＸＭＬ代表者ミドルネーム</summary>
        public string XRprsnttvMiddleNm { get; set; }
        ///<summary>ＸＭＬ代表者連絡先種別</summary>
        public string XRprsnttvPhnType { get; set; }
        ///<summary>ＸＭＬ代表者年齢</summary>
        public string XRprsnttvAge { get; set; }
        ///<summary>ＸＭＬ代表者携帯電話</summary>
        public string XRprsnttvCellularPhn { get; set; }
        ///<summary>ＸＭＬ代表者勤務先電話番号</summary>
        public string XRprsnttvOfficialPhn { get; set; }
        ///<summary>ＸＭＬ代表者年代</summary>
        public string XRprsnttvGeneration { get; set; }
        ///<summary>ＸＭＬ代表者男女区分</summary>
        public string xRprsnttvGendar { get; set; }

        ///<summary>ＸＭＬ施設ID</summary>
        public string XAccmId { get; set; }
        ///<summary>ＸＭＬ部屋割区分</summary>
        public string XAssignDiv { get; set; }
        ///<summary>ＸＭＬ男女区分</summary>
        public string XGenderDiv { get; set; }
        ///<summary>ＸＭＬ取扱区分</summary>
        public string XHandleDiv { get; set; }
        ///<summary>ＸＭＬ予約者情報区分</summary>
        public string XRsvUsrDiv { get; set; }
        ///<summary>ＸＭＬ利用区分</summary>
        public string XUseDiv { get; set; }

        ///<summary>ＳＣ受信リザプリ部屋情報カウンタ</summary>
        public int ScRcvRpRmIfCntr { get; set; }

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

                    + "XPhnNum : " + XPhnNum + "|"

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
