
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RzumeAPI.Models;

namespace RzumeAPI.Data.configuaration
{
    public class ApplicationsConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {



            builder
            .HasOne(x => x.User)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();

            builder
                .HasOne(x => x.Company)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.CompanyID)
                .HasPrincipalKey(x => x.CompanyID)
                .IsRequired();
        }
    }
}