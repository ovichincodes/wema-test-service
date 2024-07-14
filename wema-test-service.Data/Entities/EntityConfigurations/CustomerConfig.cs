namespace wema_test_service.Data.Entities.EntityConfigurations;

public class CustomerConfig : BaseEntityConfig<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        builder.HasIndex(s => s.Email).IsUnique();
        builder.HasIndex(s => s.PhoneNumber).IsUnique();

        ValueConverter<CustomerStatusEnum, string> customerStatusConverter = new(
                    s => s.ToString(),
                    s => (CustomerStatusEnum)Enum.Parse(typeof(CustomerStatusEnum), s, true));

        ValueConverter<CustomerVerificationStatusEnum, string> customerVerificationStatusConverter = new(
                    s => s.ToString(),
                    s => (CustomerVerificationStatusEnum)Enum.Parse(typeof(CustomerVerificationStatusEnum), s, true));

        builder.Property(s => s.Email).HasMaxLength(100);
        builder.Property(s => s.PhoneNumber).HasMaxLength(20);
        builder.Property(s => s.StateOfResidence).HasMaxLength(50);
        builder.Property(s => s.Lga).HasMaxLength(50);
        builder.Property(s => s.Status).HasMaxLength(20).HasConversion(customerStatusConverter);
        builder.Property(s => s.VerificationStatus).HasMaxLength(20).HasConversion(customerVerificationStatusConverter);
    }
}
