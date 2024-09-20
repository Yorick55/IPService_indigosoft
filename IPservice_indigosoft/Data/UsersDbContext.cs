namespace IPservice_indigosoft.Data
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserConnection> Connections { get; set; }
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(usr => usr.Id)
                .ValueGeneratedNever();
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserConnection>()
                .HasOne<User>(con => con.User)
                .WithMany(usr => usr.Connections)
                .HasForeignKey(con => con.UserId);
        }


    }
}
