namespace Dinex.Infra
{
    public class DinexApiContext : DbContext
    {
        public DinexApiContext(DbContextOptions<DinexApiContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<QueueIn> QueueIn { get; set; }
        public DbSet<InvestmentHistory> InvestmentHistory { get; set; }
        public DbSet<StockBroker> StockBrokers { get; set; }

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
