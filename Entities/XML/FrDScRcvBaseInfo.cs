using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    ///<summary>宿泊＿ＳＣ受信基本</summary>
    public class FrDScRcvBaseInfo : XmlBaseInfo
    {

        ///<summary>ＳＣ受信ＳＥＱ</summary>
        public int ScRcvSeq { get; set; }

        ///<summary>ＸＭＬ送り元区分</summary>
        public string XDataFr { get; set; }
        ///<summary>ＸＭＬデータ種別</summary>
        public string XDataClsfic { get; set; }
        ///<summary>ＸＭＬデータＩＤ</summary>
        public string XDataId { get; set; }
        ///<summary>ＸＭＬシステム日付</summary>
        public string XSystemDate { get; set; }
        ///<summary>ＸＭＬシステム時刻</summary>
        public string XSystemTime { get; set; }

        ///<summary>ＸＭＬ宿泊地区名</summary>
        public string XAccmArea { get; set; }
        ///<summary>ＸＭＬ宿泊施設名、消費税額</summary>
        public string XAccmNm { get; set; }
        ///<summary>ＸＭＬ宿泊施設コード</summary>
        public string XAccmCd { get; set; }
        ///<summary>ＸＭＬチェーンホテル名</summary>
        public string XChainNm { get; set; }
        ///<summary>ＸＭＬ宿泊施設担当者名</summary>
        public string XAccmPrsnInChg { get; set; }
        ///<summary>ＸＭＬ宿泊施設担当者Email</summary>
        public string XAccmEmail { get; set; }
        ///<summary>ＸＭＬ宿泊施設電話番号</summary>
        public string XAccmPhnNum { get; set; }
        ///<summary>ＸＭＬ旅行会社コード</summary>
        public string XSalesOfcCompanyCd { get; set; }
        ///<summary>ＸＭＬ旅行会社名</summary>
        public string XSalesOfcCompanyNm { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所名</summary>
        public string XSalesOfcNm { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所コード</summary>
        public string XSalesOfcCd { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所担当者名</summary>
        public string XSalesOfcPrsnInChg { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所担当者Email</summary>
        public string XSalesOfcEmail { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所電話番号</summary>
        public string XSalesOfcPhnNum { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所住所都道府県</summary>
        public string XSalesOfcStatePrvdnc { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所住所区市名</summary>
        public string XSalesOfcCityNm { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所住所町村名</summary>
        public string XSalesOfcAddressLine { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所住所番地名</summary>
        public string XSalesOfcStreetNum { get; set; }
        ///<summary>ＸＭＬ旅行会社営業所住所郵便番号</summary>
        public string XSalesOfcPostCd { get; set; }
        ///<summary>ＸＭＬ販売代理店会社名</summary>
        public string XRetailerCompanyNm { get; set; }
        ///<summary>ＸＭＬ旅行会社予約(管理)番号</summary>
        public string XTravelAgncBkngNum { get; set; }
        ///<summary>ＸＭＬ旅行会社予約受付日</summary>
        public string XTravelAgncBkngDate { get; set; }
        ///<summary>ＸＭＬ旅行会社受付時間</summary>
        public string XTravelAgncBkngTime { get; set; }
        ///<summary>ＸＭＬ通知番号</summary>
        public string XTravelAgncReportNum { get; set; }

        ///<summary>ＸＭＬ団体名または代表者氏名(半角)</summary>
        public string XGstOrGpNmSnglBt { get; set; }
        ///<summary>ＸＭＬ団体名または代表者氏名よみがな(全角)</summary>
        public string XGstOrGpNmDoubleBt { get; set; }
        ///<summary>ＸＭＬ団体名または代表者氏名 漢字</summary>
        public string XGstOrGpNmKanjiNm { get; set; }

        ///<summary>ＸＭＬチェックイン日</summary>
        public string XCheckInDate { get; set; }
        ///<summary>ＸＭＬチェックイン時間</summary>
        public string XCheckInTime { get; set; }
        ///<summary>ＸＭＬチェックアウト日</summary>
        public string XCheckOutDate { get; set; }
        ///<summary>ＸＭＬチェックアウト時間</summary>
        public string XCheckOutTime { get; set; }
        ///<summary>ＸＭＬ宿泊日数</summary>
        public int XNights { get; set; }
        ///<summary>ＸＭＬ入込方法</summary>
        public string XTrnsprt { get; set; }

        ///<summary>ＸＭＬ利用客室合計数</summary>
        public int XTtlRmCnt { get; set; }
        ///<summary>ＸＭＬお客様総合計人数</summary>
        public int XGrandTtlPaxCnt { get; set; }

        ///<summary>ＸＭＬ大人人員(男性)合計</summary>
        public int XTtlPaxMaleCnt { get; set; }
        ///<summary>ＸＭＬ大人人員(女性)合計</summary>
        public int XTtlPaxFemaleCnt { get; set; }
        ///<summary>ＸＭＬ子供A人数合計</summary>
        public int XTtlChildA70Cnt { get; set; }
        ///<summary>ＸＭＬ子供B人数合計</summary>
        public int XTtlChildB50Cnt { get; set; }
        ///<summary>ＸＭＬ子供C人数合計</summary>
        public int XTtlChildC30Cnt { get; set; }
        ///<summary>ＸＭＬ子供D人数合計</summary>
        public int XTtlCchildDNoneCnt { get; set; }
        ///<summary>ＸＭＬ子供Ｅ人数合計</summary>
        public int XTtlChildENoneCnt { get; set; }
        ///<summary>ＸＭＬ子供Ｆ人数合計</summary>
        public int XTtlChildFNoneCnt { get; set; }
        ///<summary>ＸＭＬ子供その他人数合計</summary>
        public int XTtlCchildOtherCnt { get; set; }

        ///<summary>ＸＭＬ参加形態</summary>
        public string XTypeOfGp { get; set; }
        ///<summary>ＸＭＬ予約ステータス</summary>
        public string XStatus { get; set; }
        ///<summary>ＸＭＬ企画商品区分</summary>
        public string XPackageType { get; set; }
        ///<summary>ＸＭＬ企画(パッケージ)名</summary>
        public string XPackagePlanNm { get; set; }
        ///<summary>ＸＭＬ企画(パッケージ)コード</summary>
        public string XPackagePlanCd { get; set; }
        ///<summary>ＸＭＬ企画(パッケージ)内容</summary>
        public string XPackagePlanContent { get; set; }

        ///<summary>ＸＭＬ食事条件</summary>
        public string XMealCond { get; set; }
        ///<summary>ＸＭＬ食事有無情報</summary>
        public string XSpecMealCond { get; set; }
        ///<summary>ＸＭＬ食事個所情報</summary>
        public string XMealPlace { get; set; }
        ///<summary>ＸＭＬ変更ポイント説明</summary>
        public string XModPnt { get; set; }
        ///<summary>ＸＭＬ宿泊施設取消番号</summary>
        public string XCancellationNum { get; set; }
        ///<summary>ＸＭＬ特別リクエスト</summary>
        public string XSpecialSrvcReq { get; set; }
        ///<summary>ＸＭＬ一般情報</summary>
        public string XOtherSrvcIfrm { get; set; }
        ///<summary>ＸＭＬ一般情報２</summary>
        public string XOtherSrvcIfrm2 { get; set; }
        ///<summary>ＸＭＬ詳細情報有無</summary>
        public string XFollowUpIfrm { get; set; }

        ///<summary>ＸＭＬ旅行会社作成の予約情報メール</summary>
        public string XTravelAgncEmail { get; set; }
        ///<summary>ＸＭＬ料金区分</summary>
        public string XRmrtOrprsnalrt { get; set; }
        ///<summary>ＸＭＬ税サ区分</summary>
        public string XTaxSrvcFee { get; set; }
        ///<summary>ＸＭＬ支払い方法</summary>
        public string XPayment { get; set; }
        ///<summary>ＸＭＬネット決済額</summary>
        public int XBareNetRt { get; set; }
        ///<summary>ＸＭＬクレジットカード会社名</summary>
        public string XCreditCardAuthority { get; set; }
        ///<summary>ＸＭＬクレジットカード番号</summary>
        public string XCreditCardNum { get; set; }

        ///<summary>ＸＭＬ合計宿泊料金(総額)</summary>
        public int XTtlAccmChg { get; set; }
        ///<summary>ＸＭＬ合計宿泊料金消費税</summary>
        public int XTtlAccmCnsmptTax { get; set; }
        ///<summary>ＸＭＬ合計宿泊料金入湯税</summary>
        public int XTtlAccmHotsprTax { get; set; }
        ///<summary>ＸＭＬ合計宿泊料金サービス料</summary>
        public int XTtlAccmSrvcFee { get; set; }
        ///<summary>ＸＭＬ合計その他料金</summary>
        public int XTtlAccmOtherFee { get; set; }
        ///<summary>ＸＭＬ手数料率(%)</summary>
        public double XCmmsnPercentage { get; set; }
        ///<summary>ＸＭＬ合計手数料総額</summary>
        public int XTtlAccmCmmsnAmnt { get; set; }

        ///<summary>オンライン日付</summary>
        public string OnlineDate { get; set; }
        ///<summary>バッチ日付</summary>
        public string BatchDate { get; set; }
        ///<summary>予約番号</summary>
        public string ReservationNo { get; set; }
        ///<summary>出発日</summary>
        public string CheckoutDate { get; set; }

        ///<summary>ＳＣ受信部屋情報カウンタ</summary>
        public int ScRcvRmIfCntr { get; set; }
        ///<summary>ＳＣ受信オプション情報カウンタ</summary>
        public int ScRcvOptIfCntr { get; set; }
        ///<summary>ＳＣ受信メンバ情報カウンタ</summary>
        public int ScRcvMemberCntr { get; set; }
        ///<summary>ＳＣ受信ポイント情報カウンタ</summary>
        public int ScRcvPntCntr { get; set; }
        ///<summary>ＳＣ受信デポジット情報カウンタ</summary>
        public int ScRcvDepositCntr { get; set; }
        ///<summary>ＳＣ受信エージェント情報カウンタ</summary>
        public int ScRcvAgentCntr { get; set; }

        ///<summary>ＳＣ処理済みコード</summary>
        public string ScProcessedCd { get; set; }
        ///<summary>ＳＣ処理済みメッセージ</summary>
        public string ScProcessedMessage { get; set; }
        ///<summary>ＸＭＬデータＩＤ２</summary>
        public long XDataId2 { get; set; }
        ///<summary>ＸＭＬデータ種別処理順</summary>
        public string XDataClsficOdr { get; set; }

        ///<summary>更新回数</summary>
        public int UpdateCnt { get; set; }


        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string TypeColor { get; set; }
        [NotMapped]
        public string CheckInTimeDisplay { get; set; }
        [NotMapped]
        public string CheckOutTimeDisplay { get; set; }
        [NotMapped]
        public int TotalPoint { get; set; }

    }
}
