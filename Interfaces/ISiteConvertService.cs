using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ISiteConvertService
    {

        // 情報の取得(画面表示用)
        Task<List<FrMScSiteConvertInfo>> GetSiteConvertList(FrMScSiteConvertInfo siteConvertInfo);

        // 情報取得(編集,削除用)
        Task<FrMScSiteConvertInfo> GetSiteConvertById(FrMScSiteConvertInfo siteConvertInfo);

        // 情報追加
        Task<int> AddSiteConvert(FrMScSiteConvertInfo siteConvertInfo);

        // 情報更新
        Task<int> UpdateSiteConvert(FrMScSiteConvertInfo siteConvertInfo, bool addFlag);

        // 情報削除
        Task<int> DelSiteConvert(FrMScSiteConvertInfo siteConvertInfo);

        // サイト名取得(別ページ用)
        Task<List<FrMScSiteConvertInfo>> GetSiteCodeList(string companyNo, string scCd);

        // 削除チェック
        Task<int> DeleteSiteCdCheck(FrMScSiteConvertInfo siteConvertInfo);
    }
}
