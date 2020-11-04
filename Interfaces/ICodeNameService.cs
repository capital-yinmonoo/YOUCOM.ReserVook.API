using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ICodeNameService
    {
        // 情報取得(画面表示用)
        Task<List<MstCodeNameInfo>> GetCodeNameList(MstCodeNameInfo codenameInfo);

        // 情報取得(編集,削除用)
        Task<MstCodeNameInfo> GetCodeNameById(MstCodeNameInfo codenameInfo);

        // 追加
        Task<int> AddCodeName(MstCodeNameInfo codenameInfo);

        // 更新
        Task<int> UpdateCodeName(MstCodeNameInfo codenameInfo, bool addFlag);

        // 削除
        Task<int> DelCodeName(MstCodeNameInfo codenameInfo);

        // 削除チェック
        Task<int> DeleteCodeNameCheck(MstCodeNameInfo codenameInfo);

        // リスト用情報取得
        Task<List<MstCodeNameInfo>> GetListDictionaryDataDid(string companyNo, string codeDivision);
    }
}
