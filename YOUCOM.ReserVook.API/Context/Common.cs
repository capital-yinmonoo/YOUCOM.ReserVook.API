namespace YOUCOM.ReserVook.API.Context
{
    /// <summary>
    /// 共通定数クラス
    /// </summary>
    public static class CommonConst
    {
        /// <summary>使用中</summary>
        public const string STATUS_USED = "1";

        /// <summary>未使用</summary>
        public const string STATUS_UNUSED = "9";

        /// <summary>アカウントロック</summary>
        public const string STATUS_ACCOUNT_LOCK = "8";

        /// <summary>未精算</summary>
        public const string NOT_ADJUSTMENTED = "0";

        /// <summary>精算済み</summary>
        public const string ADJUSTMENTED = "1";

        /// <summary>氏名ファイル(予約情報)の利用日</summary>
        public const string USE_DATE_EMPTY = "-";

        /// <summary>氏名ファイル(予約情報)のルートSEQ</summary>
        public const int DEFAULT_ROUTE_SEQ = 0;


        /// <summary>日時 フォーマット</summary>
        public const string DATETIME_FORMAT = "yyyyMMdd HHmmss";
        /// <summary>年月日 フォーマット</summary>
        public const string DATE_FORMAT = "yyyyMMdd";
        /// <summary>時間 フォーマット</summary>
        public const string TIME_FORMAT = "HHmmss";


        /// <summary>コード名称マスタ フォーマット</summary>
        public const string CODE_DIVISION_FORMAT = "0000";


        /// <summary>部屋状態 未アサイン</summary>
        public const string ROOMSTATUS_NOT_ASSIGN = "";

        /// <summary>部屋状態 アサイン済</summary>
        public const string ROOMSTATUS_ASSIGN = "Assign";

        /// <summary>部屋状態 滞在中</summary>
        public const string ROOMSTATUS_STAY = "Stay";

        /// <summary>部屋状態 滞在中の部屋清掃中</summary>
        public const string ROOMSTATUS_STAYCLEANING = "StayCleaning";

        /// <summary>部屋状態 滞在中の部屋清掃済</summary>
        public const string ROOMSTATUS_STAYCLEANED = "StayCleaned";

        /// <summary>部屋状態 チェックアウト</summary>
        public const string ROOMSTATUS_CO = "CO";

        /// <summary>部屋状態 清掃済</summary>
        public const string ROOMSTATUS_CLEANED = "Cleaned";

        /// <summary>部屋状態 清掃開始</summary>
        public const string ROOMSTATUS_CLEANING = "Cleaning";

        /// <summary>中抜け状態 中抜け</summary>
        public const string HOLLOWSTATUS_HOLLOW = "1";

        /// <summary>中抜け状態 デフォルト</summary>
        public const string HOLLOWSTATUS_DEFAULT = "";

        /// <summary>SCコード</summary>
        public const string SCCODE_ALL = "ALL";

        /// <summary>SCポイント割引・補助金名称 空欄</summary>
        public const string POINTS_DISCOUNTNAME_BLANK = "空欄";

        /// <summary>システムユーザー名</summary>
        public const string SYSTEM_USER_NAME = "YOUCOM";

        /// <summary>ＳＣ企画パッケージコード 空欄</summary>
        public const string PLAN_CODE_BLANK = "空欄";

        /// <summary>マスタ自動生成コード</summary>
        public const string AUTO_CONVERT_MASTER_CODE = "#";

        /// <summary>ＸＭＬデータ種別 新規</summary>
        public const string XDATACLSFIC_NEWDATA = "NewBookReport";

        /// <summary>ＳＣ処理済みコード 成功</summary>
        public const string SCPROCESSEDCD_SCCESS = "000";

        /// <summary>料理区分(当日計上)</summary>
        public const string MEAL_DIVISION_TODAY = "0";

        /// <summary>料理区分(翌日計上)</summary>
        public const string MEAL_DIVISION_NEXTDAY = "1";

    }

    /// <summary>
    /// 共通列挙クラス
    /// </summary>
    public static class CommonEnum
    {
        /// <summary>税区分</summary>
        public enum TaxDivision { NonTax = 0, InSideTax = 1, OutsideTax = 2 }

        /// <summary>税率区分</summary>
        public enum TaxrateDivision { NormalTaxrate = 1, ReducedTaxrate = 2 }

        /// <summary>サービス料区分</summary>
        public enum ServiceDivision { NonService = 0, InSideService = 1, OutsideService = 2 }

        /// <summary>禁煙喫煙区分</summary>
        public enum SmokingDivision { NoSmoking = 0, Smoking = 1 }

        /// <summary>権限区分</summary>
        public enum RoleDivision { SuperAdmin = 0, Admin = 1, User = 2 }

        /// <summary>予約状態区分</summary>
        public enum ReserveStateDivision { MainReserve = 1, CancelReserve = 2 }

        /// <summary>料理区分</summary>
        public enum MealDivision { Morning = 1, Lunch = 2, Dinner = 3 }

        /// <summary>商品区分</summary>
        public enum ItemDivision { Stay = 1, Hal, Meal, Drink, NoCategory, SetItem }

        /// <summary>セット商品区分</summary>
        public enum SetItemDivision { NormalItem = 0, SetItem }
        
        /// <summary>初期値フラグ区分</summary>
        public enum DefaultFlagDivision { OFF = 0, ON }

        /// <summary>コード区分</summary>
        public enum CodeDivision
        {
            /// <summary>権限</summary>
            Role = 1,
            /// <summary>喫煙/禁煙</summary>
            IsForbid,
            /// <summary>予約状態</summary>
            ReserveState,
            /// <summary>商品区分</summary>
            ItemDivision,
            /// <summary>食事区分</summary>
            MealDivision,
            /// <summary>部屋タイプ</summary>
            RoomType,
            /// <summary>フロア</summary>
            Floor,
            /// <summary>決済方法(TLリンカーン)</summary>
            TLPayment,
            /// <summary>清掃管理</summary>
            CleanRoomsManager,
            /// <summary>部屋タイプ区分</summary>
            RoomTypeDivision,
            /// <summary>忘れ物場所分類</summary>
            LostPlace,
            /// <summary>忘れ物保管分類</summary>
            LostStrage,
            /// <summary>TrustYou連携区分</summary>
            TrustyouConnectDiv
        }

        /// <summary>部屋タイプ区分</summary>
        public enum RoomTypeDivision { Real = 1, Fictional }

        /// <summary>予約変更履歴 処理区分</summary>
        public enum ReserveLogProcessDivision { Add = 1, Update, Delete }

        /// <summary>データベース更新結果</summary>
        /// <remarks>最終的にControllerからFrontEnd側に渡すReturn値です</remarks>
        public enum DBUpdateResult
        {
            /// <summary>OK</summary>
            Success = 0,
            /// <summary>排他エラー</summary>
            VersionError = -1,
            /// <summary>使用済みエラー(マスタ系で使用済の為削除不可)</summary>
            UsedError = -2,
            /// <summary>重複エラー</summary>
            OverlapError = -3,
            /// <summary>エラー</summary>
            Error = -99,
        }
    }
}
