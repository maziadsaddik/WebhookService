using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookService.Domain.Entities;

namespace WebhookService.Infrastructure.Persistence.Configurations
{
    public class SubscriberConfiguration : IEntityTypeConfiguration<Subscriber>
    {
        public void Configure(EntityTypeBuilder<Subscriber> builder)
        {

            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.TenantId);
            builder.HasIndex(e => new { e.TenantId, e.IsActive });
            //builder.Property(e => e.EventTypes)
            //        .HasConversion(
            //            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            //            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            //        );



            //builder.Property(x => x.Sequence).IsConcurrencyToken();

            //builder.Property(x => x.Id).HasMaxLength(Config.StringId);

            //builder.Property(x => x.PhoneNumber).HasMaxLength(14);

            //builder.Property(x => x.Value).HasPrecision(Config.DecimalPrecision, Config.DecimalScale);

            //builder.Property(x => x.Details).HasMaxLength(500);
        }
    }
}
