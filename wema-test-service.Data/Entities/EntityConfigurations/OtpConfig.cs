namespace wema_test_service.Data.Entities.EntityConfigurations;

public class OtpConfig : BaseEntityConfig<Otp>
{
    public override void Configure(EntityTypeBuilder<Otp> builder)
    {
        base.Configure(builder);
        builder.HasIndex(s => s.CreatedDate);

        ValueConverter<OtpStatusEnum, string> otpStatusConverter = new(
                    s => s.ToString(),
                    s => (OtpStatusEnum)Enum.Parse(typeof(OtpStatusEnum), s, true));

        builder.Property(s => s.CustomerId).HasMaxLength(50);
        builder.Property(s => s.PhoneNumber).HasMaxLength(20);
        builder.Property(s => s.SentDate).HasMaxLength(100);
        builder.Property(s => s.VerifiedTime).HasMaxLength(100);
        builder.Property(s => s.SmsBody).HasMaxLength(200);
        builder.Property(s => s.Status).HasMaxLength(20).HasConversion(otpStatusConverter);
    }
}
