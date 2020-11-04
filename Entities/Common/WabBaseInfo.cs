using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    public partial class WebBaseInfo
    {
        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }
        /// <summary>SCコード</summary>
        public string ScCd { get; set; }
        /// <summary>更新回数</summary>
        public int UpdateCnt { get; set; }
        /// <summary>プログラムID</summary>
        public string ProgramId { get; set; }
        /// <summary>登録担当者コード</summary>
        public string CreateClerk { get; set; }
        /// <summary>登録号機番号</summary>
        public string CreateMachineNo { get; set; }
        /// <summary>登録端末</summary>
        public string CreateMachine { get; set; }
        /// <summary>登録日時</summary>
        public string CreateDatetime { get; set; }
        /// <summary>更新担当者コード</summary>
        public string UpdateClerk { get; set; }
        /// <summary>変更号機番号</summary>
        public string UpdateMachineNo { get; set; }
        /// <summary>更新端末</summary>
        public string UpdateMachine { get; set; }
        /// <summary>更新日時</summary>
        public string UpdateDatetime { get; set; }
        /// <summary>状態</summary>
        public string Status { get; set; }

        /// 表示用
        /// <summary>サイトコントローラー名</summary>
        [NotMapped]
        public string CdName { get; set; }
    }
}
