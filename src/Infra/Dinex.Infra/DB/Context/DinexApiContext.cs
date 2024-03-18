namespace Dinex.Infra
{
    public class DinexApiContext : DbContext
    {
        public DinexApiContext(DbContextOptions<DinexApiContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<QueueIn> QueueIn { get; set; }

        #region transactions
        public DbSet<TransactionHistory> TransactionHistories { get; set; }
        public DbSet<InvestmentTransaction> InvestmentTransactions { get; set; }
        #endregion

        #region investment
        public DbSet<InvestmentHistory> InvestmentHistory { get; set; }
        public DbSet<StockBroker> StockBrokers { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=DinExDB.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Notifiable<Notification>>();
            modelBuilder.Ignore<Notification>();

            //modelBuilder.Owned<Cpf>();
            //modelBuilder.Owned<Name>();
            //modelBuilder.Owned<Email>();
            //modelBuilder.Owned<Phone>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DinexApiContext).Assembly);
        }
    }
}
