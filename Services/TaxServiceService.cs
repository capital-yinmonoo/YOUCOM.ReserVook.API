using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services
{
    public class TaxServiceService : ITaxServiceService
    {
        private DBContext _context;

        public TaxServiceService(DBContext context)
        {
            _context = context;
        }

        public async Task<MstTaxServiceInfo> GetTaxServiceByPK(MstTaxServiceInfo taxServiceInfo)
        {
            MstTaxServiceInfo info = _context.TaxServiceInfo.Where(w => w.CompanyNo == taxServiceInfo.CompanyNo && w.TaxDivision == taxServiceInfo.TaxDivision && w.ServiceDivision == taxServiceInfo.ServiceDivision && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
            return info;
        }

        public async Task<List<MstTaxServiceInfo>> GetList(MstTaxServiceInfo taxServiceInfo)
        {
            var lists = _context.TaxServiceInfo.Where(w => w.CompanyNo == taxServiceInfo.CompanyNo && w.Status != CommonConst.STATUS_UNUSED).OrderBy(n => n.DisplayName).ToList();
            return lists;
        }

        public async Task<List<TaxServiceDivisionView>> GetListView(MstTaxServiceInfo taxServiceInfo)
        {
            var lists = _context.TaxServiceInfo.Where(w => w.CompanyNo == taxServiceInfo.CompanyNo && w.Status != CommonConst.STATUS_UNUSED).OrderBy(n => n.DisplayName).ToList();
            var retLists = new List<TaxServiceDivisionView>();

            foreach (var a in lists)
            {
                var list = new TaxServiceDivisionView();
                list.TaxServiceDivision = a.TaxDivision + a.ServiceDivision;
                list.DisplayName = a.DisplayName;
                retLists.Add(list);
            }

            return retLists;
        }
    }
}
