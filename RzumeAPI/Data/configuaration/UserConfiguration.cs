
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RzumeAPI.Models;

namespace RzumeAPI.Data.configuaration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
             builder
                .HasOne(x => x.Favorites)
                .WithOne(x => x.User)
                .HasForeignKey<Favorites>(u => u.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}