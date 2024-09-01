using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using src.App_Data.Entities;

namespace src.App_Data.Configuration
{
    public class ILConfiguration : IEntityTypeConfiguration<Il>
    {
        public void Configure(EntityTypeBuilder<Il> builder)
        {
            builder.Property(p => p.IlAdi).IsRequired();

            // builder.HasIndex(e => e.IlId).IsUnique(true);            
            // builder.Property(i => i.IlId).HasColumnType("char(2)").HasMaxLength(2);
        }
    }
}
