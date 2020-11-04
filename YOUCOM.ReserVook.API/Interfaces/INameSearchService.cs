using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface INameSearchService
    {
        Task<List<NameSearchInfo>> GetNameSearchList(NameSearchCondition cond);

    }
}
