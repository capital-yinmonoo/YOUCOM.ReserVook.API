using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IUserService
    {
        // 情報取得(画面表示,削除用)
        Task<List<MstUserInfo>> GetList();

        // 情報取得(編集用)
        Task<MstUserInfo> GetById(MstUserInfo userInfo);

        // 情報追加
        Task<int> AddUser(MstUserInfo userInfo);

        // 情報更新
        Task<int> UpdateUser(MstUserInfo userInfo, bool addFlag);

        // 情報削除
        Task<int> DelUser(MstUserInfo userInfo);

        // ログイン
        LoginUser Login(string email, string pwd);

        // ログアウト
        Task<bool> Logout(string id);
    }
}
