using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services
{
    public class ScNameService : IScNameService
    {

        private DBContext _context;

        public ScNameService(DBContext context)
        {
            _context = context;
        }

        // CSサイトコード名取得(サイト変換マスタ用)
        public async Task<List<FrMScNmInfo>> GetListScName(string companyNo)
        {
            return _context.ScNameInfo
                    .Where(d => d.CompanyNo == companyNo && d.ScCd == CommonConst.SCCODE_ALL && d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.DisplayOdr).ToList();

        }

        // 各種リスト取得
        public async Task<List<FrMScNmInfo>> GetList(FrMScNmInfo cond)
        {
            return _context.ScNameInfo
                    .Where(d => d.CompanyNo == cond.CompanyNo && d.ScCd == cond.ScCd && d.ScCategoryCd == cond.ScCategoryCd && d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.DisplayOdr).ThenBy(d => d.ScSegCd).ToList();

        }
    }
}