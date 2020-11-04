namespace YOUCOM.ReserVook.API.Entities
{
    public partial class BaseInfo : BaseKey
    {
        /// <summary>状態</summary>
        public string Status { get; set; }
        /// <summary>バージョン</summary>
        public int Version { get; set; }
        /// <summary>作成者</summary>
        public string Creator { get; set; }
        /// <summary>更新者</summary>
        public string Updator { get; set; }
        /// <summary>作成日時</summary>
        public string Cdt { get; set; }
        /// <summary>更新日時</summary>
        public string Udt { get; set; }

    }
}
