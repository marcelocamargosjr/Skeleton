using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Core.Events;

namespace Skeleton.Infra.Data.Mappings
{
    public class StoredEventMap : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            // Primary Key.
            builder.HasKey(se => se.Id);

            // Properties.
            builder.Property(se => se.AggregateId)
                .IsRequired();

            builder.Property(se => se.MessageType)
                .HasColumnName("Action")
                .HasColumnType("varchar(100)");

            builder.Property(se => se.Timestamp)
                .HasColumnName("CreationDate")
                .IsRequired();

            builder.Property(se => se.Data);

            builder.Property(se => se.User);

            // Table & Column Mappings.
            builder.ToTable("StoredEvents");
        }
    }
}