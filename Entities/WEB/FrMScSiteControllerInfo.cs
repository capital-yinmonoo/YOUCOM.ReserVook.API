using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣマスタ</summary>
    public partial class FrMScSiteControllerInfo : WebBaseInfo
    {
        /// <summary>SCバージョン(1:通常)</summary>
        public string ScVer { get; set; }
        /// <summary>SC利用フラグ(0:未使用)</summary>
        public string ScUseFlg { get; set; }
        /// <summary>SCシステムID</summary>
        public string ScSystemId { get; set; }
        /// <summary>SCユーザーID</summary>
        public string ScUsrId { get; set; }
        /// <summary>SCユーザーパスワード</summary>
        public string ScUsrPassword { get; set; }
        /// <summary>SC受信通信方法</summary>
        public string ScRcvCommMethod { get; set; }
        /// <summary>SC受信通信形式</summary>
        public string ScRcvCommFormat { get; set; }
        /// <summary>SC受信通信バージョン</summary>
        public string ScRcvCommVer { get; set; }
        /// <summary>SC受信間隔</summary>
        public int ScRcvInterval { get; set; }
        /// <summary>SC新規取込フラグ(0:取り込まない)</summary>
        public string ScNewRcvFlg { get; set; }
        /// <summary>SC変更取込フラグ(0:取り込まない)</summary>
        public string ScChangeRcvFlg { get; set; }
        /// <summary>サイトコントローラー取消取込フラグ(0:取り込まない)</summary>
        public string ScCancellationRcvFlg { get; set; }
        /// <summary>SC予約受信URL</summary>
        public string ScReservationRcvUrl { get; set; }
        /// <summary>SC予約受信完了通知URL</summary>
        public string ScReservationRcvCompUrl { get; set; }
        /// <summary>SC受信ファイルパス</summary>
        public string ScRcvFilePath { get; set; }
        /// <summary>SC受信完了ファイルパス</summary>
        public string ScRcvCompFilePath { get; set; }
        /// <summary>SC受信エラーファイルパス</summary>
        public string ScRcvErrorFilePath { get; set; }
        /// <summary>SC受信ファイル名パターン</summary>
        public string ScRcvFilePattern { get; set; }
        /// <summary>SC受信ファイル拡張子</summary>
        public string ScRcvFileExt { get; set; }
        /// <summary>SC受信ファイル保存期間</summary>
        public int ScRcvFilePrsvTerm { get; set; }
        /// <summary>SC受信データ保存期間</summary>
        public int ScRcvDataPrsvTerm { get; set; }
        /// <summary>SC受信初期ブロックコード</summary>
        public string ScRcvDefBlockCd { get; set; }
        /// <summary>SC受信担当者コード</summary>
        public string ScRcvClerkCd { get; set; }
        /// <summary>SC送信間隔</summary>
        public int ScSndInterval { get; set; }
        /// <summary>SC残室送信フラグ(0:送信しない)</summary>
        public string ScRmngRmsSndFlg { get; set; }
        /// <summary>SC送信通信方法</summary>
        public string ScSndCommMethod { get; set; }
        /// <summary>SC送信通信形式</summary>
        public string ScSndCommFormat { get; set; }
        /// <summary>SC送信通信バージョン</summary>
        public string ScSndCommVer { get; set; }
        /// <summary>SC残室送信URL</summary>
        public string ScRmngRmsSndUrl { get; set; }
        /// <summary>SC送信ファイルパス</summary>
        public string ScSndFilePath { get; set; }
        /// <summary>SC送信データ保存期間</summary>
        public int ScSndDataPrsvTrem { get; set; }
        /// <summary>予約番号カナ名付加(0:何もしない 1:カナ名に追加)</summary>
        public string ScRsvNoNameAdd { get; set; }

        /// <summary>SC利用フラグ(0:未使用)</summary>
        [NotMapped]
        public string DispScUseFlg { get; set; }
        /// <summary>SC新規取込フラグ(0:取り込まない)</summary>
        [NotMapped]
        public string DispScNewRcvFlg { get; set; }
        /// <summary>サイトコントローラー取消取込フラグ(0:取り込まない)</summary>
        [NotMapped]
        public string DispScCancellationRcvFlg { get; set; }
    }
}
