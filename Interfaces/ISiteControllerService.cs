using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ISiteControllerService
    {

        // 情報の取得(一覧用)
        Task<List<FrMScSiteControllerInfo>> GetSiteControllerList(FrMScSiteControllerInfo siteControllerInfo);

        // 情報の取得(編集用)
        Task<FrMScSiteControllerInfo> GetSiteControllerById(FrMScSiteControllerInfo siteControllerInfo);

        // 情報の更新
        Task<int> UpdateSiteController(FrMScSiteControllerInfo siteControllerInfo);

    }
}
