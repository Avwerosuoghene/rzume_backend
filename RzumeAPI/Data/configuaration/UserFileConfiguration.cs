
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RzumeAPI.Models;

namespace RzumeAPI.Data.configuaration
{
    public class UserFileConfiguration : IEntityTypeConfiguration<UserFile>
    {
        public void Configure(EntityTypeBuilder<UserFile> builder)
        {
            builder
               .HasOne(x => x.User)
               .WithMany(x => x.UserFiles)
               .HasForeignKey(x => x.UserId)
               .HasPrincipalKey(x => x.Id)
               .IsRequired();
        }
    }
}