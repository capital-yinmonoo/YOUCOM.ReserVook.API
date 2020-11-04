namespace YOUCOM.ReserVook.API.Tools
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public double TokenExpiresDays { get; set; }

        public string TrustYouConnectAplPath { get; set; }

        public string TrustYouConnectTimeOutSeconds { get; set; }
    }
}
