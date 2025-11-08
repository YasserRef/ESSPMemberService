using ESSPMemberService;
using Microsoft.EntityFrameworkCore;
using ESSPMemberService.Models.Tables;
using System.Xml;


namespace ESSPMemberService.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        // Define DbSet for your entities, e.g.,
        //public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseOracle(_configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.MigrationsAssembly("ESSPMemberService");
                    options.UseRelationalNulls();
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<V_T_DIFFMEMBER>().HasKey(e => new { e.F_CODE, e.F_YEAR }); // Composite Key
            modelBuilder.Entity<ResortsNotReserved>().HasKey(e => new { e.F_HCODE, e.F_WEEK, e.F_FCODE }); // Composite Key
            //.HasIndex(e => new { e.F_CODE, e.F_YEAR }); // Composite Index

            // modelBuilder.Entity<V_SPENDDATE>().HasKey(e => new { e.F_CODE, e.F_YEAR,e.F_MEMBER });
            // modelBuilder.Entity<V_SPENDDATE_SELF>().HasKey(e => new { e.F_CODE, e.F_YEAR, e.F_MEMBER });
            modelBuilder.Entity<V_SPENDDATE>().HasNoKey().ToView("V_SPENDDATE");
            modelBuilder.Entity<V_SPENDDATE_SELF>().HasNoKey().ToView("V_SPENDDATE_SELF");
            modelBuilder.Entity<V_RESERVATION_RESORT>().HasNoKey().ToView("V_RESERVATION_RESORT");
            modelBuilder.Entity<ResortsNotReserved>().ToView("V_RESORT_NOT_RESERVED");

            modelBuilder.Entity<V_SYSCODE>().HasKey(e => new { e.F_CODE, e.F_TYPE }); // Composite Key
            modelBuilder.Entity<V_SYSCODE>().ToView("V_SYSCODE");
            modelBuilder.Entity<V_PAYMENT_MAIN>().HasNoKey().ToView("V_PAYMENT_MAIN");
            


            base.OnModelCreating(modelBuilder);
        }
        public DbSet<V_T_TRAINING_DESCRIPTION> V_T_TRAINING_DESCRIPTION { get; set; } = default!;
        public DbSet<V_T_BRANCH_DESCRIPTION> V_T_BRANCH_DESCRIPTION { get; set; } = default!;
        public DbSet<V_T_GOV> V_T_GOV { get; set; } = default!;
        
        public DbSet<T_PAYMENT> T_PAYMENT { get; set; } = default!;
        public DbSet<T_PAYMENT_MAIN> T_PAYMENT_MAIN { get; set; } = default!;
        public DbSet<T_PAYMENT_MEM_CARD> T_PAYMENT_MEM_CARD { get; set; } = default!;
        public DbSet<T_PAYMENT_METHOD> T_PAYMENT_METHOD { get; set; } = default!;        
        public DbSet<T_PAYMENT_COMPANY> T_PAYMENT_COMPANY { get; set; } = default!;
        public DbSet<V_T_DIFFMEMBER> V_T_DIFFMEMBER { get; set; } = default!;
        public DbSet<V_MEMBER_INFO> V_MEMBER_INFO { get; set; } = default!;
        public DbSet<V_T_PENALTY> V_T_PENALTY { get; set; } = default!;
        public DbSet<V_SPENDDATE> V_SPENDDATE { get; set; } = default!;
        public DbSet<V_SPENDDATE_SELF> V_SPENDDATE_SELF { get; set; } = default!;
        public DbSet<T_REQUESTS> T_REQUESTS { get; set; } = default!;
        public DbSet<V_REQUESTS> V_REQUESTS { get; set; } = default!;
        public DbSet<V_RESERVATION_RESORT> V_RESERVATION_RESORT { get; set; } = default!;
        public DbSet<ResortsNotReserved> V_RESORT_NOT_RESERVED { get; set; } = default!;
        public DbSet<V_USERS> V_USERS { get; set; } = default!;
        public DbSet<V_FLAT_PIC> V_FLAT_PICS { get; set; } = default!;
        public DbSet<V_SYSCODE> V_SYSCODE { get; set; } = default!;
        public DbSet<V_PAYMENT_MAIN> V_PAYMENT_MAIN { get; set; } = default!;
        public DbSet<T_PAYMENT_DETAIL> T_PAYMENT_DETAIL { get; set; } = default!;        
        public DbSet<T_NEWS> T_NEWS { get; set; } = default!;
    }

}
