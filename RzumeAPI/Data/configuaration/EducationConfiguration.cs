using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RzumeAPI.Models;

namespace RzumeAPI.Data.configuaration
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {
            builder
               .HasOne(x => x.User)
               .WithMany(u => u.Education)
               .HasForeignKey(c => c.UserId)
               .IsRequired();
        }
    }
}