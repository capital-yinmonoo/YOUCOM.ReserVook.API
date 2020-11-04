using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    public class LoginUser : MstUserInfo
    {
        public string JwtToken { get; set; }
        public string LoginResult { get; set; }
        public int LoginResultValue { get; set; }
    }

    public class LoginUserMemoryCacheModel
    {
        public string JwtTokenId { get; set; }
        public System.DateTime LastDateTime { get; set; }
    }

    public class LoginInfo : MstUserInfo
    {
        public int TryCount { get; set; }
    }
}
