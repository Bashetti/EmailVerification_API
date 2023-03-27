using Microsoft.EntityFrameworkCore;

namespace WebApplication1
{
    public class EmailDBContext:DbContext
    {
        public DbSet<Emails> Emails { get; set; }
        public DbSet<EmailLogDB> EmailLogs { get; set; }
        public EmailDBContext(DbContextOptions<EmailDBContext> options)
           : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Emails>().ToTable(nameof(Emails));
            modelBuilder.Entity<EmailLogDB>().ToTable(nameof(EmailLogs));
        }
    }
}
