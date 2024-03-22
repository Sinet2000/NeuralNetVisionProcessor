namespace NetVisionProc.Data
{
    public class AppMainDbContext : DbContext
    {
        public AppMainDbContext(DbContextOptions<AppMainDbContext> options)
            : base(options)
        {
        }

        // public DbSet<TplTestModel> TplTestModels => Set<TplTestModel>();
        
        protected static void UpdateTableNamesToSingularForm(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.IsMappedToJson())
                {
                    continue;
                }

                entityType.SetTableName(entityType.DisplayName());
            }
        }

        protected sealed override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 6);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            UpdateTableNamesToSingularForm(builder);
        }
    }
}