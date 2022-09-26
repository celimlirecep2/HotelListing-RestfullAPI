using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
                new Country
                {
                    Id = 1,
                    Name = "Türkiye",
                    ShortName = "Tr"
                },
                new Country
                {
                    Id = 2,
                    Name = "İngiltere",
                    ShortName = "En"
                },
                new Country
                {
                    Id = 3,
                    Name = "Fransa",
                    ShortName = "Fr"
                },
                new Country
                {
                    Id = 4,
                    Name = "Bahama",
                    ShortName = "Bs"
                }
                );
        }
    }
}
