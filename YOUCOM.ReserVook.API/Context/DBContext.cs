using Microsoft.EntityFrameworkCore;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Context
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }
        #region MstTable
        public virtual DbSet<MstAgentInfo> AgentInfo { get; set; }
        public virtual DbSet<MstCodeDivisionInfo> CodeDivisionInfo { get; set; }
        public virtual DbSet<MstCodeNameInfo> CodeNameInfo { get; set; }
        public virtual DbSet<MstCompanyInfo> CompanyInfo { get; set; }
        public virtual DbSet<MstDenominationInfo> DenominationInfo { get; set; }
        public virtual DbSet<MstItemInfo> ItemInfo { get; set; }
        public virtual DbSet<MstRoomsInfo> RoomsInfo { get; set; }
        public virtual DbSet<MstTaxRateInfo> TaxRateInfo { get; set; }
        public virtual DbSet<MstTaxServiceInfo> TaxServiceInfo { get; set; }
        public virtual DbSet<MstUserInfo> UserInfo { get; set; }
        public virtual DbSet<MstStateInfo> StateInfo { get; set; }
        public virtual DbSet<MstFacilityInfo> FacilityInfo { get; set; }
        public virtual DbSet<MstSetItemInfo> SetItemInfo { get; set; }
        public virtual DbSet<MstTrustyouInfo> SetTrustyouInfo { get; set; }

        public virtual DbSet<MstCompanyGroupInfo> CompanyGroupInfo { get; set; }
        public virtual DbSet<MstCustomerInfo> CustomerInfo { get; set; }
        #endregion
        #region TrnTable
        public virtual DbSet<TrnDepositDetailsInfo> DepositDetailsInfo { get; set; }
        public virtual DbSet<TrnNameFileInfo> NameFileInfo { get; set; }
        public virtual DbSet<TrnReserveAssignInfo> ReserveAssignInfo { get; set; }
        public virtual DbSet<TrnReserveBasicInfo> ReserveBasicInfo { get; set; }
        public virtual DbSet<TrnReserveNoteInfo> ReserveNoteInfo { get; set; }
        public virtual DbSet<TrnReserveRoomtypeInfo> ReserveRoomtypeInfo { get; set; }
        public virtual DbSet<TrnSalesDetailsInfo> SalesDetailsInfo { get; set; }
        public virtual DbSet<TrnReserveFacilityInfo> ReserveFacilityInfo { get; set; }
        public virtual DbSet<TrnLostItemsBaseInfo> LostItemsBaseInfo { get; set; }
        public virtual DbSet<TrnLostItemsPictureInfo> LostItemsPictureInfo { get; set; }
        public virtual DbSet<TrnTrustyouInfo> TrustyouInfo { get; set; }
        public virtual DbSet<TrnTrustyouLogInfo> TrustyouLogInfo { get; set; }
        public virtual DbSet<TrnCleaningManagementReceiveInfo> CleanManagementInfo { get; set; }
        public virtual DbSet<TrnUseResultsInfo> UseResultsInfo { get; set; }
        public virtual DbSet<TrnReserveLogInfo> ReserveLogInfo { get; set; }
        #endregion
        #region WebTable
        public virtual DbSet<FrMScNmInfo> ScNameInfo { get; set; }
        public virtual DbSet<FrMScPaymentConvertInfo> PaymentConvertInfo { get; set; }
        public virtual DbSet<FrMScPlanConvertInfo> PlanConvertInfo { get; set; }
        public virtual DbSet<FrMScPointConvertInfo> PointConvertInfo { get; set; }
        public virtual DbSet<FrMScRemarksConvertInfo> RemarksConvertInfo { get; set; }
        public virtual DbSet<FrMScRmtypeConvertInfo> RoomTypeConvertInfo { get; set; }
        public virtual DbSet<FrMScSiteControllerInfo> SiteControllerInfo { get; set; }
        public virtual DbSet<FrMScSiteConvertInfo> SiteConvertInfo { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //↓Document\01基本設計\DB構造設計の各シートに簡易生成クエリ有
            #region MstTable
            modelBuilder.Entity<MstCompanyInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo }).HasName("mst_company_PK");
                entity.ToTable("mst_company");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(100);
                entity.Property(e => e.ZipCode).HasColumnName("zip_code").HasMaxLength(8);
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.PhoneNo).HasColumnName("phone_no").HasMaxLength(20);
                entity.Property(e => e.BillingAddress).HasColumnName("billing_address").HasMaxLength(60);
                entity.Property(e => e.LogoData).HasColumnName("logo_data");
                entity.Property(e => e.ContentType).HasColumnName("content_type").HasMaxLength(20);
                entity.Property(e => e.ServiceRate).HasColumnName("service_rate");
                entity.Property(e => e.LastReserveNo).HasColumnName("last_reserve_no").HasMaxLength(8);
                entity.Property(e => e.LastCustomerNo).HasColumnName("last_customer_no").HasMaxLength(10);
                entity.Property(e => e.LastBillNo).HasColumnName("last_bill_no");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
                entity.Property(e => e.LostFlg).HasColumnName("lost_flg").HasMaxLength(1);
                entity.Property(e => e.TrustyouConnectDiv).HasColumnName("trustyou_connect_div").HasMaxLength(1);
                entity.Property(e => e.SavePeriod).HasColumnName("save_period");
                entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");
                entity.Property(e => e.CompanyGroupId).HasColumnName("company_group_id");
                entity.Property(e => e.eRegicardConnectdiv).HasColumnName("eregicard_connect_div");
                entity.Property(e => e.eRegicardConnectHotelCode).HasColumnName("eregicard_connect_hotelcode");
                entity.Property(e => e.eRegicardConnectPassword).HasColumnName("eregicard_connect_password");
            });
            modelBuilder.Entity<MstUserInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.UserEmail }).HasName("mst_user_PK");
                entity.ToTable("mst_user");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.UserEmail).HasColumnName("user_email").IsRequired().HasMaxLength(60);
                entity.Property(e => e.Password).HasColumnName("password").IsRequired().HasMaxLength(40);
                entity.Property(e => e.UserName).HasColumnName("user_name").IsRequired().HasMaxLength(40);
                entity.Property(e => e.RoleDivision).HasColumnName("role_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.LostFlg).HasColumnName("lost_flg").HasMaxLength(1);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
                entity.Property(e => e.LostFlg).HasColumnName("lost_flg").HasMaxLength(1);
            });
            modelBuilder.Entity<MstItemInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ItemCode }).HasName("mst_item_PK");
                entity.ToTable("mst_item");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ItemCode).HasColumnName("item_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ItemDivision).HasColumnName("item_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.MealDivision).HasColumnName("meal_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.ItemName).HasColumnName("item_name").IsRequired().HasMaxLength(60);
                entity.Property(e => e.ItemKana).HasColumnName("item_kana").IsRequired().HasMaxLength(60);
                entity.Property(e => e.PrintName).HasColumnName("print_name").HasMaxLength(40);
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
                entity.Property(e => e.TaxDivision).HasColumnName("tax_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.TaxrateDivision).HasColumnName("taxrate_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.ServiceDivision).HasColumnName("service_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstCodeDivisionInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.DivisionCode }).HasName("mst_code_division_PK");
                entity.ToTable("mst_code_division");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.DivisionCode).HasColumnName("division_code").IsRequired().HasMaxLength(4);
                entity.Property(e => e.DivisionName).HasColumnName("division_name").IsRequired().HasMaxLength(20);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstCodeNameInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.DivisionCode, e.Code }).HasName("mst_code_name_PK");
                entity.ToTable("mst_code_name");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.DivisionCode).HasColumnName("division_code").IsRequired().HasMaxLength(4);
                entity.Property(e => e.Code).HasColumnName("code").IsRequired().HasMaxLength(4);
                entity.Property(e => e.CodeName).HasColumnName("code_name").HasMaxLength(40);
                entity.Property(e => e.CodeValue).HasColumnName("code_value").HasMaxLength(4);
                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstRoomsInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.RoomNo }).HasName("mst_rooms_PK");
                entity.ToTable("mst_rooms");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.RoomNo).HasColumnName("room_no").IsRequired().HasMaxLength(4);
                entity.Property(e => e.RoomName).HasColumnName("room_name").IsRequired().HasMaxLength(40);
                entity.Property(e => e.RoomTypeCode).HasColumnName("room_type_code").IsRequired().HasMaxLength(4);
                entity.Property(e => e.Floor).HasColumnName("floor").HasMaxLength(4);
                entity.Property(e => e.Remarks).HasColumnName("remarks");
                entity.Property(e => e.SmokingDivision).HasColumnName("smoking_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.RowIndex).HasColumnName("row_index");
                entity.Property(e => e.ColumnIndex).HasColumnName("column_index");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstTaxServiceInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.TaxDivision, e.ServiceDivision }).HasName("mst_tax_service_PK");
                entity.ToTable("mst_tax_service");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.DisplayName).HasColumnName("display_name").IsRequired().HasMaxLength(16);
                entity.Property(e => e.TaxDivision).HasColumnName("tax_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.ServiceDivision).HasColumnName("service_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstTaxRateInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.TaxRate, e.BeginDate, e.EndDate }).HasName("mst_tax_rate_PK");
                entity.ToTable("mst_tax_rate");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.TaxRate).HasColumnName("tax_rate").IsRequired();
                entity.Property(e => e.BeginDate).HasColumnName("begin_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.EndDate).HasColumnName("end_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.TaxrateDivision).HasColumnName("taxrate_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version");
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstAgentInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.AgentCode });
                entity.ToTable("mst_agent");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.AgentCode).HasColumnName("agent_code").IsRequired().HasMaxLength(16);
                entity.Property(e => e.AgentName).HasColumnName("agent_name").HasMaxLength(1);
                entity.Property(e => e.Remarks).HasColumnName("remarks");
                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstDenominationInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.DenominationCode }).HasName("mst_denomination_PK");
                entity.ToTable("mst_denomination");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.DenominationCode).HasColumnName("denomination_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.DenominationName).HasColumnName("denomination_name").IsRequired().HasMaxLength(60);
                entity.Property(e => e.PrintName).HasColumnName("print_name").HasMaxLength(40);
                entity.Property(e => e.ReceiptDiv).HasColumnName("receipt_div").HasMaxLength(1);
                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            modelBuilder.Entity<MstFacilityInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.FacilityCode }).HasName("mst_facility_PK");
                entity.ToTable("mst_facility");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.FacilityCode).HasColumnName("facility_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.FacilityName).HasColumnName("facility_name").IsRequired().HasMaxLength(40);
                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstStateInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ItemStateCode }).HasName("mst_state_pk");
                entity.ToTable("mst_state");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ItemStateCode).HasColumnName("item_state_code").IsRequired().HasMaxLength(2);
                entity.Property(e => e.ItemStateName).HasColumnName("item_state_name").HasMaxLength(20);
                entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(1);
                entity.Property(e => e.DefaultFlagSearch).HasColumnName("default_flag_search").HasMaxLength(1);
                entity.Property(e => e.DefaultFlagEntry).HasColumnName("default_flag_entry").HasMaxLength(1);
                entity.Property(e => e.OrderNo).HasColumnName("order_no");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstSetItemInfo>(entity => {
                entity.HasKey(e => new { e.CompanyNo, e.SetItemCode, e.Seq }).HasName("mst_set_item_pk");
                entity.ToTable("mst_set_item");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.SetItemCode).HasColumnName("set_item_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.Seq).HasColumnName("seq").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ItemCode).HasColumnName("item_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
                entity.Property(e => e.TaxrateDivision).HasColumnName("taxrate_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.ServiceDivision).HasColumnName("service_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<MstTrustyouInfo>(entity => {
                entity.HasKey(e => new { e.CompanyNo }).HasName("mst_trustyou_data_pk");
                entity.ToTable("mst_trustyou_data");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.HotelCode ).HasColumnName("hotel_code").HasMaxLength(100);
                entity.Property(e => e.SendUrl).HasColumnName("send_url").HasMaxLength(300);
                entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(100);
                entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(100);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            modelBuilder.Entity<MstCompanyGroupInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyGroupId }).HasName("mst_company_group_PK");
                entity.ToTable("mst_company_group");
                entity.Property(e => e.CompanyGroupId).HasColumnName("company_group_id").IsRequired().HasMaxLength(10);
                entity.Property(e => e.CompanyGroupName).HasColumnName("company_group_name").HasMaxLength(100);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);

            });

            modelBuilder.Entity<MstCustomerInfo>(entity => {
                entity.HasKey(e => new { e.CompanyNo, e.CustomerNo }).HasName("mst_customer_pk");
                entity.ToTable("mst_customer");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.CustomerNo).HasColumnName("customer_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
                entity.Property(e => e.CustomerKana).HasColumnName("customer_kana").HasMaxLength(100);
                entity.Property(e => e.ZipCode).HasColumnName("zip_code").HasMaxLength(8);
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.PhoneNo).HasColumnName("phone_no").HasMaxLength(20);
                entity.Property(e => e.MobilePhoneNo).HasColumnName("mobile_phone_no").HasMaxLength(20);
                entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(60);
                entity.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(100);
                entity.Property(e => e.Remarks1).HasColumnName("remarks1");
                entity.Property(e => e.Remarks2).HasColumnName("remarks2");
                entity.Property(e => e.Remarks3).HasColumnName("remarks3");
                entity.Property(e => e.Remarks4).HasColumnName("remarks4");
                entity.Property(e => e.Remarks5).HasColumnName("remarks5");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version");
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            #endregion
            #region TrnTable
            modelBuilder.Entity<TrnReserveBasicInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo }).HasName("trn_reserve_basic_PK");
                entity.ToTable("trn_reserve_basic");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.ReserveDate).HasColumnName("reserve_date").HasMaxLength(8);
                entity.Property(e => e.ReserveStateDivision).HasColumnName("reserve_state_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.ArrivalDate).HasColumnName("arrival_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.DepartureDate).HasColumnName("departure_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.StayDays).HasColumnName("stay_days").IsRequired();
                entity.Property(e => e.MemberMale).HasColumnName("member_male").IsRequired();
                entity.Property(e => e.MemberFemale).HasColumnName("member_female").IsRequired();
                entity.Property(e => e.MemberChildA).HasColumnName("member_child_a").IsRequired();
                entity.Property(e => e.MemberChildB).HasColumnName("member_child_b").IsRequired();
                entity.Property(e => e.MemberChildC).HasColumnName("member_child_c").IsRequired();
                entity.Property(e => e.AdjustmentFlag).HasColumnName("adjustment_flag").HasMaxLength(1);
                entity.Property(e => e.CustomerNo).HasColumnName("customer_no").HasMaxLength(10);
                entity.Property(e => e.AgentCode).HasColumnName("agent_code").HasMaxLength(16);
                entity.Property(e => e.AgentRemarks).HasColumnName("agent_remarks");
                entity.Property(e => e.XTravelAgncBkngNum).HasColumnName("x_travel_agnc_bkng_num").HasMaxLength(255);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnReserveRoomtypeInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.UseDate, e.RoomtypeSeq }).HasName("trn_reserve_roomtype_PK");
                entity.ToTable("trn_reserve_roomtype");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.UseDate).HasColumnName("use_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.RoomtypeCode).HasColumnName("roomtype_code").IsRequired().HasMaxLength(4);
                entity.Property(e => e.RoomtypeSeq).HasColumnName("roomtype_seq").IsRequired();
                entity.Property(e => e.Rooms).HasColumnName("rooms").IsRequired();
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnReserveAssignInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.UseDate, e.RouteSEQ }).HasName("trn_reserve_assign_PK");
                entity.ToTable("trn_reserve_assign");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.UseDate).HasColumnName("use_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.RouteSEQ).HasColumnName("route_seq").IsRequired();
                entity.Property(e => e.RoomNo).HasColumnName("room_no").HasMaxLength(4);
                entity.Property(e => e.RoomtypeCode).HasColumnName("roomtype_code").IsRequired().HasMaxLength(4);
                entity.Property(e => e.OrgRoomtypeCode).HasColumnName("org_roomtype_code").IsRequired().HasMaxLength(4);
                entity.Property(e => e.RoomtypeSeq).HasColumnName("roomtype_seq").IsRequired();
                entity.Property(e => e.RoomStateClass).HasColumnName("room_state_class").HasMaxLength(20);
                entity.Property(e => e.GuestName).HasColumnName("guest_name").HasMaxLength(100);
                entity.Property(e => e.MemberMale).HasColumnName("member_male").IsRequired();
                entity.Property(e => e.MemberFemale).HasColumnName("member_female").IsRequired();
                entity.Property(e => e.MemberChildA).HasColumnName("member_child_a").IsRequired();
                entity.Property(e => e.MemberChildB).HasColumnName("member_child_b").IsRequired();
                entity.Property(e => e.MemberChildC).HasColumnName("member_child_c").IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(60);
                entity.Property(e => e.CleaningInstruction).HasColumnName("cleaning_instruction");
                entity.Property(e => e.CleaningRemarks).HasColumnName("cleaning_remarks");
                entity.Property(e => e.HollowStateClass).HasColumnName("hollow_state_class");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnSalesDetailsInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.DetailsSeq }).HasName("trn_sales_details_PK");
                entity.ToTable("trn_sales_details");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.DetailsSeq).HasColumnName("details_seq");
                entity.Property(e => e.ItemDivision).HasColumnName("item_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.MealDivision).HasColumnName("meal_division").HasMaxLength(1);
                entity.Property(e => e.UseDate).HasColumnName("use_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.ItemCode).HasColumnName("item_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.PrintName).HasColumnName("print_name").HasMaxLength(40);
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
                entity.Property(e => e.ItemNumberM).HasColumnName("item_number_m");
                entity.Property(e => e.ItemNumberF).HasColumnName("item_number_f");
                entity.Property(e => e.ItemNumberC).HasColumnName("item_number_c");
                entity.Property(e => e.AmountPrice).HasColumnName("amount_price");
                entity.Property(e => e.InsideTaxPrice).HasColumnName("inside_tax_price");
                entity.Property(e => e.InsideServicePrice).HasColumnName("inside_service_price");
                entity.Property(e => e.OutsideServicePrice).HasColumnName("outside_service_price");
                entity.Property(e => e.TaxRate).HasColumnName("tax_rate");
                entity.Property(e => e.BillSeparateSeq).HasColumnName("bill_separate_seq").IsRequired();
                entity.Property(e => e.BillNo).HasColumnName("bill_no").HasMaxLength(10);
                entity.Property(e => e.TaxDivision).HasColumnName("tax_division").HasMaxLength(1);
                entity.Property(e => e.TaxRateDivision).HasColumnName("taxrate_division").HasMaxLength(1);
                entity.Property(e => e.ServiceDivision).HasColumnName("service_division").HasMaxLength(1);
                entity.Property(e => e.SetItemDivision).HasColumnName("set_item_division").HasMaxLength(1);
                entity.Property(e => e.SetItemSeq).HasColumnName("set_item_seq");
                entity.Property(e => e.AdjustmentFlag).HasColumnName("adjustment_flag").HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
            });
            modelBuilder.Entity<TrnDepositDetailsInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.BillSeparateSeq, e.DetailsSeq }).HasName("trn_deposit_details_PK");
                entity.ToTable("trn_deposit_details");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.BillSeparateSeq).HasColumnName("bill_separate_seq").IsRequired();
                entity.Property(e => e.BillNo).HasColumnName("bill_no").HasMaxLength(10);
                entity.Property(e => e.DetailsSeq).HasColumnName("details_seq").IsRequired();
                entity.Property(e => e.DepositDate).HasColumnName("deposit_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.DenominationCode).HasColumnName("denomination_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.PrintName).HasColumnName("print_name").HasMaxLength(40);
                entity.Property(e => e.AmountPrice).HasColumnName("amount_price");
                entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(200);
                entity.Property(e => e.AdjustmentFlag).HasColumnName("adjustment_flag").HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnReserveNoteInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.NoteSeq }).HasName("trn_reserve_note_PK");
                entity.ToTable("trn_reserve_note");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.NoteSeq).HasColumnName("note_seq").IsRequired();
                entity.Property(e => e.Remarks).HasColumnName("remarks");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnNameFileInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.UseDate, e.RouteSEQ, e.NameSeq }).HasName("trn_name_file_PK");
                entity.ToTable("trn_name_file");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.NameSeq).HasColumnName("name_seq").IsRequired();

                entity.Property(e => e.UseDate).HasColumnName("use_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.RouteSEQ).HasColumnName("route_seq").IsRequired();

                entity.Property(e => e.GuestName).HasColumnName("guest_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.GuestKana).HasColumnName("guest_kana").IsRequired().HasMaxLength(100);
                entity.Property(e => e.ZipCode).HasColumnName("zip_code").HasMaxLength(8);
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.PhoneNo).HasColumnName("phone_no").HasMaxLength(20);
                entity.Property(e => e.MobilePhoneNo).HasColumnName("mobile_phone_no").HasMaxLength(20);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(60);
                entity.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(100);
                entity.Property(e => e.CustomerNo).HasColumnName("customer_no").HasMaxLength(10);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            modelBuilder.Entity<TrnReserveFacilityInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.FacilityCode, e.UseDate, e.FacilitySeq }).HasName("trn_reserve_facility_PK");
                entity.ToTable("trn_reserve_facility");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.FacilityCode).HasColumnName("facility_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.UseDate).HasColumnName("use_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.FacilitySeq).HasColumnName("facility_seq").IsRequired();
                entity.Property(e => e.StartTime).HasColumnName("start_time").HasMaxLength(4);
                entity.Property(e => e.EndTime).HasColumnName("end_time").HasMaxLength(4);
                entity.Property(e => e.FacilityMember).HasColumnName("facility_member");
                entity.Property(e => e.FacilityRemarks).HasColumnName("facility_remarks");
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").HasMaxLength(8);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnLostItemsBaseInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ManagementNo }).HasName("trn_lost_items_base_pk");
                entity.ToTable("trn_lost_items_base");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ManagementNo).HasColumnName("management_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.ItemState).HasColumnName("item_state").HasMaxLength(2);
                entity.Property(e => e.ItemCategory).HasColumnName("item_category").HasMaxLength(2);
                entity.Property(e => e.ItemName).HasColumnName("item_name").HasMaxLength(50);
                entity.Property(e => e.FoundDate).HasColumnName("found_date").HasMaxLength(8);
                entity.Property(e => e.FoundTime).HasColumnName("found_time").HasMaxLength(4);
                entity.Property(e => e.FoundPlace).HasColumnName("found_place");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.SearchWord).HasColumnName("search_word");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
                entity.Property(e => e.FoundPlaceCode).HasColumnName("found_place_code").HasMaxLength(15);
                entity.Property(e => e.StorageCode).HasColumnName("storage_code").HasMaxLength(15);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").HasMaxLength(8);
                entity.Property(e => e.RoomNo).HasColumnName("room_no").HasMaxLength(4);
            });
            modelBuilder.Entity<TrnLostItemsPictureInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ManagementNo, e.FileSeq }).HasName("trn_lost_items_picture_pk");
                entity.ToTable("trn_lost_items_picture");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ManagementNo).HasColumnName("management_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.FileSeq).HasColumnName("file_seq");
                entity.Property(e => e.ContentType).HasColumnName("content_type").HasMaxLength(20);
                entity.Property(e => e.FileName).HasColumnName("file_name");
                entity.Property(e => e.BinaryData).HasColumnName("binary_data");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            modelBuilder.Entity<TrnTrustyouInfo>(entity => {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.ArrivalDate, e.DepartureDate, e.RoomNo }).HasName("trn_trustyou_data_pk");
                entity.ToTable("trn_trustyou_data");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.ArrivalDate).HasColumnName("arrival_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.DepartureDate).HasColumnName("departure_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.RoomNo).HasColumnName("room_no").IsRequired().HasMaxLength(4);
                entity.Property(e => e.GuestName).HasColumnName("guest_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(60);
                entity.Property(e => e.LanguageCode).HasColumnName("language_code").HasMaxLength(4);
                entity.Property(e => e.SendDate).HasColumnName("send_date").HasMaxLength(8);
                entity.Property(e => e.SendTime).HasColumnName("send_time").HasMaxLength(6);
                entity.Property(e => e.SendResult).HasColumnName("send_result").HasMaxLength(2);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            modelBuilder.Entity<TrnTrustyouLogInfo>(entity => {
                entity.HasKey(e => new { e.CompanyNo, e.LogSeq }).HasName("trn_trustyou_log_pk");
                entity.ToTable("trn_trustyou_log");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.LogSeq).HasColumnName("log_seq").IsRequired();
                entity.Property(e => e.ProcessDate).HasColumnName("process_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.ProcessTime).HasColumnName("process_time").IsRequired().HasMaxLength(6);
                entity.Property(e => e.ProcessUser).HasColumnName("process_user").HasMaxLength(40);
                entity.Property(e => e.LogMessage).HasColumnName("log_message").IsRequired().HasMaxLength(100);
                entity.Property(e => e.ErrorCode).HasColumnName("error_code").HasMaxLength(4);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnCleaningManagementReceiveInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo,e.Seq }).HasName("trn_cleaning_management_re_PK");
                entity.ToTable("trn_cleaning_management_receive");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.Seq).HasColumnName("seq").IsRequired();
                entity.Property(e => e.MessageType).HasColumnName("message_type").HasMaxLength(2);
                entity.Property(e => e.RoomNo).HasColumnName("room_no").HasMaxLength(4);
                entity.Property(e => e.Contents).HasColumnName("contents").HasMaxLength(255);
                entity.Property(e => e.ProcessingFlag).HasColumnName("processing_flag").HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });
            modelBuilder.Entity<TrnUseResultsInfo>(entity => {
                entity.HasKey(e => new { e.CompanyNo, e.CustomerNo, e.ReserveNo }).HasName("trn_use_results_PK");
                entity.ToTable("trn_use_results");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.CustomerNo).HasColumnName("customer_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.ArrivalDate).HasColumnName("arrival_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.DepartureDate).HasColumnName("departure_date").IsRequired().HasMaxLength(8);
                entity.Property(e => e.UseAmount).HasColumnName("use_amount");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            modelBuilder.Entity<TrnReserveLogInfo>(entity => {
                entity.HasKey(e => new { e.CompanyNo, e.ReserveNo, e.ReserveLogSeq }).HasName("trn_reserve_log_PK");
                entity.ToTable("trn_reserve_log");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ReserveNo).HasColumnName("reserve_no").IsRequired().HasMaxLength(8);
                entity.Property(e => e.ReserveLogSeq).HasColumnName("reserve_log_seq").IsRequired();
                entity.Property(e => e.SeqGroup).HasColumnName("seq_group").IsRequired();
                entity.Property(e => e.ProcessDivision).HasColumnName("process_division").IsRequired().HasMaxLength(1);
                entity.Property(e => e.ChangeItem).HasColumnName("change_item").HasMaxLength(40);
                entity.Property(e => e.BeforeValue).HasColumnName("before_value").HasMaxLength(100);
                entity.Property(e => e.AfterValue).HasColumnName("after_value").HasMaxLength(100);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);
                entity.Property(e => e.Version).HasColumnName("version").IsRequired();
                entity.Property(e => e.Creator).HasColumnName("creator").HasMaxLength(40);
                entity.Property(e => e.Updator).HasColumnName("updator").HasMaxLength(40);
                entity.Property(e => e.Cdt).HasColumnName("cdt").HasMaxLength(15);
                entity.Property(e => e.Udt).HasColumnName("udt").HasMaxLength(15);
            });

            #endregion
            #region WebTable
            modelBuilder.Entity<FrMScNmInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ScCd, e.ScCategoryCd, e.ScSegCd }).HasName("mgrpk_fr_m_sc_nm");
                entity.ToTable("fr_m_sc_nm");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCategoryCd).HasColumnName("sc_category_cd").IsRequired().HasMaxLength(2);
                entity.Property(e => e.ScSegCd).HasColumnName("sc_seg_cd").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Content1).HasColumnName("content_1").HasMaxLength(20);
                entity.Property(e => e.Content2).HasColumnName("content_2").HasMaxLength(20);
                entity.Property(e => e.DisplayOdr).HasColumnName("display_odr");
                entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

            });
            modelBuilder.Entity<FrMScPaymentConvertInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ScCd, e.ScSiteCd, e.ScPaymentOpts }).HasName("mgrpk_fr_m_sc_payment_convert");
                entity.ToTable("fr_m_sc_payment_convert");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScSiteCd).HasColumnName("sc_site_cd").IsRequired().HasMaxLength(30);
                entity.Property(e => e.ScPaymentOpts).HasColumnName("sc_payment_opts").IsRequired().HasMaxLength(60);
                entity.Property(e => e.DenominationCode).HasColumnName("denomination_code").HasMaxLength(10);
                entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

            });
            modelBuilder.Entity<FrMScPlanConvertInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ScPackagePlanCd, e.ScMealCond, e.ScSpecMealCond, e.ScCd }).HasName("mgrpk_fr_m_sc_plan_convert");
                entity.ToTable("fr_m_sc_plan_convert");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScPackagePlanCd).HasColumnName("sc_package_plan_cd").IsRequired().HasMaxLength(30);
                entity.Property(e => e.ScMealCond).HasColumnName("sc_meal_cond").IsRequired().HasMaxLength(50);
                entity.Property(e => e.ScSpecMealCond).HasColumnName("sc_spec_meal_cond").HasMaxLength(10);
                entity.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(10);
                entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

            });
            modelBuilder.Entity<FrMScPointConvertInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ScCd, e.ScSiteCd, e.ScPntsDiscntNm }).HasName("mgrpk_fr_m_sc_point_convert");
                entity.ToTable("fr_m_sc_point_convert");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScSiteCd).HasColumnName("sc_site_cd").IsRequired().HasMaxLength(30);
                entity.Property(e => e.ScPntsDiscntNm).HasColumnName("sc_pnts_discnt_nm").IsRequired().HasMaxLength(50);
                entity.Property(e => e.DenominationCode).HasColumnName("denomination_code").HasMaxLength(10);
                entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

            });
            modelBuilder.Entity<FrMScRemarksConvertInfo>(entity =>
             {
                 entity.HasKey(e => new { e.CompanyNo, e.ScCd, e.ScXClmn }).HasName("mgrpk_fr_m_sc_remarks_conv");
                 entity.ToTable("fr_m_sc_remarks_convert");
                 entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                 entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                 entity.Property(e => e.ScXClmn).HasColumnName("sc_x_clmn").IsRequired().HasMaxLength(30);
                 entity.Property(e => e.ScXClmnKanji).HasColumnName("sc_x_clmn_kanji").HasMaxLength(30);
                 entity.Property(e => e.ScRemarksSetLocation).HasColumnName("sc_remarks_set_location");
                 entity.Property(e => e.ScRemarksPriorityOdr).HasColumnName("sc_remarks_priority_odr");
                 entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                 entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                 entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                 entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                 entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                 entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                 entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                 entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                 entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                 entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                 entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

             });
            modelBuilder.Entity<FrMScRmtypeConvertInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ScCd, e.ScRmtypeCd }).HasName("mgrpk_fr_m_sc_rmtype_conv");
                entity.ToTable("fr_m_sc_rmtype_convert");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScRmtypeCd).HasColumnName("sc_rmtype_cd").IsRequired().HasMaxLength(50);
                entity.Property(e => e.RmtypeCd).HasColumnName("rmtype_cd").HasMaxLength(3);
                entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

            });
            modelBuilder.Entity<FrMScSiteControllerInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ScCd }).HasName("mgrpk_fr_m_sc_site_controller");
                entity.ToTable("fr_m_sc_site_controller");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScVer).HasColumnName("sc_ver").HasMaxLength(5);
                entity.Property(e => e.ScUseFlg).HasColumnName("sc_use_flg").IsRequired().HasMaxLength(1);
                entity.Property(e => e.ScSystemId).HasColumnName("sc_system_id").HasMaxLength(50);
                entity.Property(e => e.ScUsrId).HasColumnName("sc_usr_id").HasMaxLength(50);
                entity.Property(e => e.ScUsrPassword).HasColumnName("sc_usr_password").HasMaxLength(50);
                entity.Property(e => e.ScRcvCommMethod).HasColumnName("sc_rcv_comm_method").HasMaxLength(10);
                entity.Property(e => e.ScRcvCommFormat).HasColumnName("sc_rcv_comm_format").HasMaxLength(5);
                entity.Property(e => e.ScRcvCommVer).HasColumnName("sc_rcv_comm_ver").HasMaxLength(5);
                entity.Property(e => e.ScRcvInterval).HasColumnName("sc_rcv_interval");
                entity.Property(e => e.ScNewRcvFlg).HasColumnName("sc_new_rcv_flg").HasMaxLength(1);
                entity.Property(e => e.ScChangeRcvFlg).HasColumnName("sc_change_rcv_flg").HasMaxLength(1);
                entity.Property(e => e.ScCancellationRcvFlg).HasColumnName("sc_cancellation_rcv_flg").HasMaxLength(1);
                entity.Property(e => e.ScReservationRcvUrl).HasColumnName("sc_reservation_rcv_url").HasMaxLength(100);
                entity.Property(e => e.ScReservationRcvCompUrl).HasColumnName("sc_reservation_rcv_comp_url").HasMaxLength(100);
                entity.Property(e => e.ScRcvFilePath).HasColumnName("sc_rcv_file_path").HasMaxLength(100);
                entity.Property(e => e.ScRcvCompFilePath).HasColumnName("sc_rcv_comp_file_path").HasMaxLength(100);
                entity.Property(e => e.ScRcvErrorFilePath).HasColumnName("sc_rcv_error_file_path").HasMaxLength(100);
                entity.Property(e => e.ScRcvFilePattern).HasColumnName("sc_rcv_file_pattern").HasMaxLength(20);
                entity.Property(e => e.ScRcvFileExt).HasColumnName("sc_rcv_file_ext").HasMaxLength(5);
                entity.Property(e => e.ScRcvFilePrsvTerm).HasColumnName("sc_rcv_file_prsv_term");
                entity.Property(e => e.ScRcvDataPrsvTerm).HasColumnName("sc_rcv_data_prsv_term");
                entity.Property(e => e.ScRcvDefBlockCd).HasColumnName("sc_rcv_def_block_cd").HasMaxLength(4);
                entity.Property(e => e.ScRcvClerkCd).HasColumnName("sc_rcv_clerk_cd").HasMaxLength(3);
                entity.Property(e => e.ScSndInterval).HasColumnName("sc_snd_interval");
                entity.Property(e => e.ScRmngRmsSndFlg).HasColumnName("sc_rmng_rms_snd_flg").HasMaxLength(1);
                entity.Property(e => e.ScSndCommMethod).HasColumnName("sc_snd_comm_method").HasMaxLength(10);
                entity.Property(e => e.ScSndCommFormat).HasColumnName("sc_snd_comm_format").HasMaxLength(5);
                entity.Property(e => e.ScSndCommVer).HasColumnName("sc_snd_comm_ver").HasMaxLength(5);
                entity.Property(e => e.ScRmngRmsSndUrl).HasColumnName("sc_rmng_rms_snd_url").HasMaxLength(100);
                entity.Property(e => e.ScSndFilePath).HasColumnName("sc_snd_file_path").HasMaxLength(100);
                entity.Property(e => e.ScSndDataPrsvTrem).HasColumnName("sc_snd_data_prsv_term");
                entity.Property(e => e.ScRsvNoNameAdd).HasColumnName("sc_rsv_no_name_add").HasMaxLength(1);
                entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

            });
            modelBuilder.Entity<FrMScSiteConvertInfo>(entity =>
            {
                entity.HasKey(e => new { e.CompanyNo, e.ScCd, e.ScSiteCd }).HasName("mgrpk_fr_m_sc_site_conv");
                entity.ToTable("fr_m_sc_site_convert");
                entity.Property(e => e.CompanyNo).HasColumnName("company_no").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScCd).HasColumnName("sc_cd").IsRequired().HasMaxLength(10);
                entity.Property(e => e.ScSiteCd).HasColumnName("sc_site_cd").IsRequired().HasMaxLength(30);
                entity.Property(e => e.ScSiteNm).HasColumnName("sc_site_nm").HasMaxLength(30);
                entity.Property(e => e.TravelAgncCd).HasColumnName("travel_agnc_cd").HasMaxLength(6);
                entity.Property(e => e.ScPositionMan).HasColumnName("sc_position_man").HasMaxLength(1);
                entity.Property(e => e.ScPositionWoman).HasColumnName("sc_position_woman").HasMaxLength(1);
                entity.Property(e => e.ScPositionChildA).HasColumnName("sc_position_child_a").HasMaxLength(1);
                entity.Property(e => e.ScPositionChildB).HasColumnName("sc_position_child_b").HasMaxLength(1);
                entity.Property(e => e.ScPositionChildC).HasColumnName("sc_position_child_c").HasMaxLength(1);
                entity.Property(e => e.ScPositionChildD).HasColumnName("sc_position_child_d").HasMaxLength(1);
                entity.Property(e => e.ScPositionChildE).HasColumnName("sc_position_child_e").HasMaxLength(1);
                entity.Property(e => e.ScPositionChildF).HasColumnName("sc_position_child_f").HasMaxLength(1);
                entity.Property(e => e.ScPositionChildOther).HasColumnName("sc_position_child_other").HasMaxLength(1);
                entity.Property(e => e.ScPersonCalcSeg).HasColumnName("sc_person_calc_seg").HasMaxLength(1);
                entity.Property(e => e.UpdateCnt).HasColumnName("update_cnt").IsRequired();
                entity.Property(e => e.ProgramId).HasColumnName("program_id").HasMaxLength(20);
                entity.Property(e => e.CreateClerk).HasColumnName("create_clerk").HasMaxLength(40);
                entity.Property(e => e.CreateMachineNo).HasColumnName("create_machine_no").HasMaxLength(2);
                entity.Property(e => e.CreateMachine).HasColumnName("create_machine").HasMaxLength(32);
                entity.Property(e => e.CreateDatetime).HasColumnName("create_datetime").HasMaxLength(15);
                entity.Property(e => e.UpdateClerk).HasColumnName("update_clerk").HasMaxLength(40);
                entity.Property(e => e.UpdateMachineNo).HasColumnName("update_machine_no").HasMaxLength(2);
                entity.Property(e => e.UpdateMachine).HasColumnName("update_machine").HasMaxLength(32);
                entity.Property(e => e.UpdateDatetime).HasColumnName("update_datetime").HasMaxLength(15);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(1);

            });
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
