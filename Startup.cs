using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Services;
using YOUCOM.ReserVook.API.Tools;

namespace YOUCOM.ReserVook.API
{
    public class Startup
    {
        //TODO: 動作に問題がないのを確認後、コメントアウト部分を削除
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public Newtonsoft.Json.JsonSerializerSettings SerializerSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors();
            services.AddCors(options =>
            {
                //    //options.AddDefaultPolicy(
                //    //    builder => builder
                //    //        .AllowAnyMethod()
                //    //        .AllowAnyHeader()
                //    //        .WithOrigins(new string[] { "http://localhost:4200" })
                //    //);

                options.AddPolicy("AllowSpecificOrigin",
                 builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(new string[] { "http://localhost:4200" })
                );
            });
            services.AddControllers();


            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddScoped<IUserService, UserService>();

            //#region Cors
            //var urls = Configuration["AppSettings:Cors"].Split(',');

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowSpecificOrigin",
            //        builder =>
            //        {
            //            builder.WithOrigins(urls).
            //                    AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
            //        });
            //});
            //#endregion

            //services.AddControllers();


            //services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);  
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddDbContext<DBContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            #region services
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<ICompanyService, CompanyService>();

            services.AddTransient<ICodeNameService, CodeNameService>();

            services.AddTransient<IRoomsService, RoomsService>();

            services.AddTransient<IItemService, ItemService>();

            services.AddTransient<ITaxRateService, TaxRateService>();

            services.AddTransient<ITaxServiceService, TaxServiceService>();

            services.AddTransient<IWebReserveService, WebReserveService>();

            services.AddTransient<IReserveService, ReserveService>();

            services.AddTransient<ISalesReportService, SalesReportService>();

            services.AddTransient<ILedgerService, LedgerService>();

            services.AddTransient<IDenominationService, DenominationService>();

            services.AddTransient<IAgentService, AgentService>();

            services.AddTransient<IScNameService, ScNameService>();

            services.AddTransient<IPaymentConvertService, PaymentConvertService>();

            services.AddTransient<IPlanConvertService, PlanConvertService>();

            services.AddTransient<IPointConvertService, PointConvertService>();

            services.AddTransient<IRemarksConvertService, RemarksConvertService>();

            services.AddTransient<IRoomTypeConvertService, RoomTypeConvertService>();

            services.AddTransient<ISiteControllerService, SiteControllerService>();

            services.AddTransient<ISiteConvertService, SiteConvertService>();

            services.AddTransient<IBillService, BillService>();

            services.AddTransient<IWebReserveService, WebReserveService>();

            services.AddTransient<IConfirmIncomeService, ConfirmIncomeService>();

            services.AddTransient<INameSearchService, NameSearchService>();

            services.AddTransient<IFacilityService, FacilityService>();

            services.AddTransient<IStateService, StateService>();

            services.AddTransient<INameFileService, NameFileService>();

            services.AddTransient<ISetItemService, SetItemService>();
            
            services.AddTransient<ILostItemListService, LostItemListService>();
            
            services.AddTransient<ILostItemDetailService, LostItemDetailService>();

            services.AddTransient<ITrustyouService, TrustyouService>();

            services.AddTransient<IRoomInfoService, RoomInfoService>();

            services.AddTransient<ICompanyGroupService, CompanyGroupService>();

            services.AddTransient<ICustomerService, CustomerService>();

            services.AddTransient<IUseResultsService, UseResultsService>();

            services.AddTransient<IReserveLogService, ReserveLogService>();

            services.AddTransient<IDishReportService, DishReportService>();

            services.AddTransient<IDataImportService, DataImportService>();

            services.AddTransient<IDataExportService, DataExportService>();
            #endregion

            #region authentication
            //services.AddMemoryCache();

            //var appSettingsSection = Configuration.GetSection("AppSettings");
            //services.Configure<AppSettings>(appSettingsSection);

            ////configure jwt authentication
            //var appSettings = appSettingsSection.Get<AppSettings>();
            //var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            #region UseCors
            //app.UseCors("AllowSpecificOrigin");
            #endregion
            //app.UseMiddleware(typeof(AuthorizeMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors();

        }
    }
}
