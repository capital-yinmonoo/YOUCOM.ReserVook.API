namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信メンバ情報</summary>
    public class FrDScRcvMemberIfInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }
        ///<summary>ＳＣ受信メンバ情報ＳＥＱ</summary>
        public int ScRcvMemberSeq { get; set; }
        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }

        ///<summary>ＸＭＬ予約者・会員名漢字</summary>
        public string XUsrNm { get; set; }
        ///<summary>ＸＭＬ予約者・会員名カタカナ</summary>
        public string XUsrKana { get; set; }
        ///<summary>ＸＭＬ予約者・会員電話番号</summary>
        public string XUsrTel { get; set; }
        ///<summary>ＸＭＬ予約者・会員Email</summary>
        public string XUsrMailAddr { get; set; }
        ///<summary>ＸＭＬ予約者・会員郵便番号</summary>
        public string XUsrZip { get; set; }
        ///<summary>ＸＭＬ予約者・会員住所 </summary>
        public string XUsrAddr { get; set; }
        ///<summary>ＸＭＬ予約者・会員会社</summary>
        public string XUsrCorp { get; set; }
        ///<summary>ＸＭＬ予約者・会員所属部署</summary>
        public string XUsrDep { get; set; }
        ///<summary>ＸＭＬ予約者・会員番号</summary>
        public string XUsrId { get; set; }

        ///<summary>ＸＭＬ付与ポイント</summary>
        public int XUsrGivingPnts { get; set; }
        ///<summary>ＸＭＬ使用ポイント</summary>
        public int XUsrUsePnts { get; set; }

        ///<summary>ＸＭＬ会員種別</summary>
        public string XUsrType { get; set; }
        ///<summary>ＸＭＬ予約者生年月日</summary>
        public string XUsrDateOfBirth { get; set; }
        ///<summary>ＸＭＬ予約者性別</summary>
        public string XUsrGendar { get; set; }
        ///<summary>ＸＭＬ予約者緊急連絡先番号(携帯等)</summary>
        public string XUsrEmergencyPhnNum { get; set; }
        ///<summary>ＸＭＬ予約者ご職業</summary>
        public string XUsrOccupation { get; set; }
        ///<summary>ＸＭＬ宿泊施設からのメルマガ受信希望(予約者)</summary>
        public string XUsrMailMgznFrAccm { get; set; }
        ///<summary>ＸＭＬ予約者役職</summary>
        public string XUsrPost { get; set; }
        ///<summary>ＸＭＬ予約者勤務先住所</summary>
        public string XUsrOfcAddr { get; set; }
        ///<summary>ＸＭＬ予約者勤務先電話番号</summary>
        public string XUsrOfcPhn { get; set; }
        ///<summary>ＸＭＬ累計ポイント</summary>
        public int XUsrTtlPnt { get; set; }
        ///<summary>ＸＭＬ予約者・会員会社コード</summary>
        public string XUsrCorpId { get; set; }
        ///<summary>ＸＭＬ予約者・会員会社名カナ</summary>
        public string XUsrCorpKana { get; set; }
        ///<summary>ＸＭＬ予約者・会員勤務先郵便番号</summary>
        public string XMemberOfcPostCd { get; set; }

        ///<summary>ＸＭＬお客様からの要望</summary>
        public string XGstReq { get; set; }
        ///<summary>ＸＭＬ予約補足情報</summary>
        public string XAdditionalIfrm { get; set; }
        ///<summary>ＸＭＬ合計宿泊料金に対するサービス料</summary>
        public int XTtlAccmSrvcChg { get; set; }
        ///<summary>ＸＭＬ割引後の総額</summary>
        public int XTtlAccmDeclPnts { get; set; }
        ///<summary>ＸＭＬ宿泊者請求額</summary>
        public int XAmntClaimed { get; set; }

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
                    + "ScRcvMemberSeq : " + ScRcvMemberSeq.ToString() + "|"
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
