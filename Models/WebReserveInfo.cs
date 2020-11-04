using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    public class WebReserveInfo
    {
        public FrDScRcvBaseInfo FrDScRcvBase;

        public FrDScRcvAgentIfInfo FrDScRcvAgentIf;

        public FrDScRcvMemberIfInfo FrDScRcvMemberIf;

        public List<FrDScRcvOptIfInfo> FrDScRcvOptIf;

        public List<FrDScRcvPntIfInfo> FrDScRcvPntIf;

        public List<FrDScRcvRmIfInfo> FrDScRcvRmIf;

        public List<FrDScRcvRmRtIfInfo> FrDScRcvRmRtIf;

        public FrDScRcvXmlInfo FrDScRcvXml;

        public FrDScRcvRpBaseInfo FrDScRcvRpBase;

        public List<FrDScRcvRpRmIfInfo> FrDScRcvRpRmIf;

        public List<FrDScRcvRpRmRtIfInfo> FrDScRcvRpRmRtIf;


        public WebReserveInfo()
        {
            FrDScRcvBase = new FrDScRcvBaseInfo();
            FrDScRcvAgentIf = new FrDScRcvAgentIfInfo();
            FrDScRcvMemberIf = new FrDScRcvMemberIfInfo();
            FrDScRcvOptIf = new List<FrDScRcvOptIfInfo>();
            FrDScRcvPntIf = new List<FrDScRcvPntIfInfo>();
            FrDScRcvRmIf = new List<FrDScRcvRmIfInfo>();
            FrDScRcvRmRtIf = new List<FrDScRcvRmRtIfInfo>();
            FrDScRcvXml = new FrDScRcvXmlInfo();

            FrDScRcvRpBase = new FrDScRcvRpBaseInfo();
            FrDScRcvRpRmIf = new List<FrDScRcvRpRmIfInfo>();
            FrDScRcvRpRmRtIf = new List<FrDScRcvRpRmRtIfInfo>();
        }
    }
}
