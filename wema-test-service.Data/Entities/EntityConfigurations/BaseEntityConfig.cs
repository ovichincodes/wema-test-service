namespace wema_test_service.Data.Entities.EntityConfigurations;

public abstract class BaseEntityConfig<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.HasIndex(x => x.CreatedDate);

        builder.Property(b => b.Id).HasMaxLength(50);//.HasDefaultValueSql("newid()");
        builder.Property(b => b.CreatedDate).HasMaxLength(100);
        builder.Property(b => b.ModifiedDate).HasMaxLength(100);
    }
}
