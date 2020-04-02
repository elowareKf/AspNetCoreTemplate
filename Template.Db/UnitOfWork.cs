namespace Template.Db {
    public class UnitOfWork : IUnitOfWork{
        private readonly string _connectionString;
        public readonly ProjectContext Context;

        public UnitOfWork(string connectionString) {
            _connectionString = connectionString;
            Context = new ProjectContext(_connectionString);
            
            // TODO: Initialize Repositories:
            Users = new UserRepository(Context.User, Context);

        }
        
        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public UserRepository Users { get; set; }
    }
}