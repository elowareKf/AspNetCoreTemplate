using Microsoft.EntityFrameworkCore;
using Template.Models;

namespace Template.Db {
    public class ProjectContext : DbContext {
        private readonly string _connectionString;

        /// <summary>
        /// For CLI usages only
        /// </summary>
        public ProjectContext() {
            
        }

        public ProjectContext(string connectionString) {
            _connectionString = connectionString;
        }

        public DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer(_connectionString ??
                              "Data Source=.,1434;Initial Catalog=SecuritySystem;user=sa;pwd=SqlServer.1");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            
            //TODO: Include IEntityTypeConfiguration<Model> here
        }
    }
}