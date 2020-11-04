using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    public partial class WebBaseInfo
    {
        /// <summary>��Дԍ�</summary>
        public string CompanyNo { get; set; }
        /// <summary>SC�R�[�h</summary>
        public string ScCd { get; set; }
        /// <summary>�X�V��</summary>
        public int UpdateCnt { get; set; }
        /// <summary>�v���O����ID</summary>
        public string ProgramId { get; set; }
        /// <summary>�o�^�S���҃R�[�h</summary>
        public string CreateClerk { get; set; }
        /// <summary>�o�^���@�ԍ�</summary>
        public string CreateMachineNo { get; set; }
        /// <summary>�o�^�[��</summary>
        public string CreateMachine { get; set; }
        /// <summary>�o�^����</summary>
        public string CreateDatetime { get; set; }
        /// <summary>�X�V�S���҃R�[�h</summary>
        public string UpdateClerk { get; set; }
        /// <summary>�ύX���@�ԍ�</summary>
        public string UpdateMachineNo { get; set; }
        /// <summary>�X�V�[��</summary>
        public string UpdateMachine { get; set; }
        /// <summary>�X�V����</summary>
        public string UpdateDatetime { get; set; }
        /// <summary>���</summary>
        public string Status { get; set; }

        /// �\���p
        /// <summary>�T�C�g�R���g���[���[��</summary>
        [NotMapped]
        public string CdName { get; set; }
    }
}
