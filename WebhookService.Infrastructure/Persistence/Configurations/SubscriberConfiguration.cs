using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
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

            builder.Property(e => e.EventTypes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );
        }
    }
}
