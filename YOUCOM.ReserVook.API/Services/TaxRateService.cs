using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services
{
    public class TaxRateService : ITaxRateService
    {
        private DBContext _context;

        public TaxRateService(DBContext context)
        {
            _context = context;
        }

        //public async Task<MstTaxRateInfo> GetTaxRateByPK(MstTaxRateInfo taxRateInfo)
        //{
        //    MstTaxRateInfo info = _context.TaxRateInfo.Where(r => r.CompanyNo == taxRateInfo.CompanyNo 
        //                                                       && r.TaxrateDivision == taxRateInfo.TaxrateDivision 
        //                                                       && r.Status != CommonConst.STATUS_UNUSED
        //                                                    ).SingleOrDefault();
        //    return info;
        //}

        public async Task<List<MstTaxRateInfo>> GetList(BaseInfo cond)
        {
            var lists = _context.TaxRateInfo.Where(r => r.CompanyNo == cond.CompanyNo
                                                     && r.Status != CommonConst.STATUS_UNUSED
                                                  ).OrderBy(o => o.BeginDate)
                                                   .ThenBy(p => p.EndDate)
                                                   .ThenBy(q => q.TaxrateDivision)
                                                   .ToList();
            return lists;
        }

    }
}
