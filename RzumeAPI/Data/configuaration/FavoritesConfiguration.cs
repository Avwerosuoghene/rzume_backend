
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RzumeAPI.Models;

namespace RzumeAPI.Data.configuaration
{
    public class FavoritesConfiguration : IEntityTypeConfiguration<Favorites>
    {
        public void Configure(EntityTypeBuilder<Favorites> builder)
        {
            builder
                .HasMany(x => x.Applications)
                .WithOne(x => x.Favorites)
                .HasForeignKey(x => x.ApplicationID)
                .HasPrincipalKey(x => x.FavoritesID)
                .IsRequired();
        }
    }
}