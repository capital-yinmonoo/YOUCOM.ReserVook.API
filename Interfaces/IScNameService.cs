using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IScNameService
    {
        // SCサイトコード名取得
        Task<List<FrMScNmInfo>> GetListScName(string companyNo);

        // 情報取得
        Task<List<FrMScNmInfo>> GetList(FrMScNmInfo cond);
    }
}
