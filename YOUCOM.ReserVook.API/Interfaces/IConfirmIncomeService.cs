using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IConfirmIncomeService
    {

        //Task<List<CustomerInfo>> GetCustomerInfoList(DateInfo date);
        Task<List<ConfirmIncomeInfo>> GetIncomeList(string companyNo, string date);

    }
}
