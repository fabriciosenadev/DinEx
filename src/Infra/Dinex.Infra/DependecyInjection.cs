namespace Dinex.Infra
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<DinexApiContext>();

            #region infra service
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISendEmailService, SendEmailService>();
            #endregion

            #region generic repositories
            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            #endregion

            #region respository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IQueueInRepository, QueueInRepository>();
            services.AddScoped<IInvestmentHistoryRepository, InvestmentHistoryRepository>();
            services.AddScoped<IStockBrokerRepository, StockBrokerRepository>();
            services.AddScoped<IAssetRepository, AssetRepository>();
            #endregion

            return services;
        }
    }
}
