
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RzumeAPI.Models;

namespace RzumeAPI.Data.configuaration
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
            builder
                .HasOne(x => x.User)
                .WithMany(u => u.Experience)
                .HasForeignKey(c => c.UserId)
                .IsRequired();
        }
    }
}