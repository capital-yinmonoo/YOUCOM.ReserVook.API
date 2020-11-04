namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>
    /// サイトコントローラー連携　共通情報
    /// </summary>
    public class XmlBaseInfo
    {

        ///<summary>会社番号</summary>
        public string CompanyNo { get; set; }

        ///<summary>ＳＣコード</summary>
        public string ScCd { get; set; }



        ///<summary>プログラムＩＤ</summary>
        public string ProgramId { get; set; }
        ///<summary>登録担当者コード</summary>
        public string CreateClerkCd { get; set; }
        ///<summary>登録号機番号</summary>
        public string CreateMachineNo { get; set; }
        ///<summary>登録端末</summary>
        public string CreateMachine { get; set; }
        ///<summary>登録日時 : YYYYMMDD hhmmss</summary>
        public string CreateDatetime { get; set; }
        ///<summary>更新担当者コード</summary>
        public string UpdateClerkCd { get; set; }
        ///<summary>変更号機番号</summary>
        public string UpdateMachineNo { get; set; }
        ///<summary>更新端末</summary>
        public string UpdateMachine { get; set; }
        ///<summary>更新日時 : YYYYMMDD hhmmss</summary>
        public string UpdateDatetime { get; set; }

    }
}
