namespace YOUCOM.ReserVook.API.Entities
{
    public partial class BaseInfo : BaseKey
    {
        /// <summary>���</summary>
        public string Status { get; set; }
        /// <summary>�o�[�W����</summary>
        public int Version { get; set; }
        /// <summary>�쐬��</summary>
        public string Creator { get; set; }
        /// <summary>�X�V��</summary>
        public string Updator { get; set; }
        /// <summary>�쐬����</summary>
        public string Cdt { get; set; }
        /// <summary>�X�V����</summary>
        public string Udt { get; set; }

    }
}
