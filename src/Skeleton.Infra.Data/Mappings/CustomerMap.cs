using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Models;

namespace Skeleton.Infra.Data.Mappings
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Primary Key.
            builder.HasKey(c => c.Id);

            // Properties.
            builder.Property(c => c.CreationDate)
                .IsRequired();

            builder.Property(c => c.UpdateDate);

            builder.Property(c => c.Name)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.BirthDate)
                .IsRequired();

            // Table & Column Mappings.
            builder.ToTable("Customers");
        }
    }
}