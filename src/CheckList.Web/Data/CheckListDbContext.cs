namespace CheckList.Web.Data;

public class CheckListDbContext : DbContext
{
    public CheckListDbContext(DbContextOptions<CheckListDbContext> options) : base(options) { }

    public DbSet<TemplateSet> TemplateSets => Set<TemplateSet>();
    public DbSet<TemplateList> TemplateLists => Set<TemplateList>();
    public DbSet<TemplateCategory> TemplateCategories => Set<TemplateCategory>();
    public DbSet<TemplateAction> TemplateActions => Set<TemplateAction>();
    public DbSet<CheckSet> CheckSets => Set<CheckSet>();
    public DbSet<CheckListEntity> CheckLists => Set<CheckListEntity>();
    public DbSet<CheckCategory> CheckCategories => Set<CheckCategory>();
    public DbSet<CheckAction> CheckActions => Set<CheckAction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TemplateSet
        modelBuilder.Entity<TemplateSet>(e =>
        {
            e.ToTable("TemplateSet");
            e.HasKey(x => x.SetId);
            e.Property(x => x.SetName).HasMaxLength(255).IsRequired();
            e.Property(x => x.SetDscr).HasMaxLength(1000).IsRequired();
            e.Property(x => x.OwnerName).HasMaxLength(256).IsRequired();
            e.Property(x => x.ActiveInd).HasMaxLength(1).IsRequired().HasDefaultValue("Y");
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
        });

        // TemplateList
        modelBuilder.Entity<TemplateList>(e =>
        {
            e.ToTable("TemplateList");
            e.HasKey(x => x.ListId);
            e.Property(x => x.ListName).HasMaxLength(255).IsRequired();
            e.Property(x => x.ListDscr).HasMaxLength(1000).IsRequired();
            e.Property(x => x.ActiveInd).HasMaxLength(1).IsRequired().HasDefaultValue("Y");
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.HasOne(x => x.Set).WithMany(s => s.TemplateLists).HasForeignKey(x => x.SetId).OnDelete(DeleteBehavior.Cascade);
        });

        // TemplateCategory
        modelBuilder.Entity<TemplateCategory>(e =>
        {
            e.ToTable("TemplateCategory");
            e.HasKey(x => x.CategoryId);
            e.Property(x => x.CategoryText).HasMaxLength(255).IsRequired();
            e.Property(x => x.CategoryDscr).HasMaxLength(1000);
            e.Property(x => x.ActiveInd).HasMaxLength(1).IsRequired().HasDefaultValue("Y");
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.HasOne(x => x.List).WithMany(l => l.TemplateCategories).HasForeignKey(x => x.ListId).OnDelete(DeleteBehavior.Cascade);
        });

        // TemplateAction
        modelBuilder.Entity<TemplateAction>(e =>
        {
            e.ToTable("TemplateAction");
            e.HasKey(x => x.ActionId);
            e.Property(x => x.ActionText).HasMaxLength(255);
            e.Property(x => x.ActionDscr).HasMaxLength(1000);
            e.Property(x => x.CompleteInd).HasMaxLength(1);
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.HasOne(x => x.Category).WithMany(c => c.TemplateActions).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade);
        });

        // CheckSet
        modelBuilder.Entity<CheckSet>(e =>
        {
            e.ToTable("CheckSet");
            e.HasKey(x => x.SetId);
            e.Property(x => x.SetName).HasMaxLength(255).IsRequired();
            e.Property(x => x.SetDscr).HasMaxLength(1000);
            e.Property(x => x.OwnerName).HasMaxLength(256).IsRequired();
            e.Property(x => x.ActiveInd).HasMaxLength(1).IsRequired().HasDefaultValue("Y");
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.HasOne(x => x.TemplateSet).WithMany(t => t.CheckSets).HasForeignKey(x => x.TemplateSetId).OnDelete(DeleteBehavior.NoAction);
        });

        // CheckList
        modelBuilder.Entity<CheckListEntity>(e =>
        {
            e.ToTable("CheckList");
            e.HasKey(x => x.ListId);
            e.Property(x => x.ListName).HasMaxLength(255).IsRequired();
            e.Property(x => x.ListDscr).HasMaxLength(1000);
            e.Property(x => x.ActiveInd).HasMaxLength(1).IsRequired().HasDefaultValue("Y");
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.HasOne(x => x.Set).WithMany(s => s.CheckLists).HasForeignKey(x => x.SetId).OnDelete(DeleteBehavior.Cascade);
        });

        // CheckCategory
        modelBuilder.Entity<CheckCategory>(e =>
        {
            e.ToTable("CheckCategory");
            e.HasKey(x => x.CategoryId);
            e.Property(x => x.CategoryText).HasMaxLength(255).IsRequired();
            e.Property(x => x.CategoryDscr).HasMaxLength(1000);
            e.Property(x => x.ActiveInd).HasMaxLength(1).IsRequired().HasDefaultValue("Y");
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.HasOne(x => x.List).WithMany(l => l.CheckCategories).HasForeignKey(x => x.ListId).OnDelete(DeleteBehavior.Cascade);
        });

        // CheckAction
        modelBuilder.Entity<CheckAction>(e =>
        {
            e.ToTable("CheckAction");
            e.HasKey(x => x.ActionId);
            e.Property(x => x.ActionText).HasMaxLength(255).IsRequired();
            e.Property(x => x.ActionDscr).HasMaxLength(1000);
            e.Property(x => x.CompleteInd).HasMaxLength(1).IsRequired().HasDefaultValue("N");
            e.Property(x => x.SortOrder).HasDefaultValue(50);
            e.Property(x => x.CreateDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.CreateUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.Property(x => x.ChangeDateTime).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ChangeUserName).HasMaxLength(255).HasDefaultValue("UNKNOWN");
            e.HasOne(x => x.Category).WithMany(c => c.CheckActions).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
