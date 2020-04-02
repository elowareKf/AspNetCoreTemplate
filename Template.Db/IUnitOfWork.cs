namespace Template.Db {
    public interface IUnitOfWork {
        void SaveChanges();

        UserRepository Users { get; set; }
        // TODO: declare Repositories
    }
}