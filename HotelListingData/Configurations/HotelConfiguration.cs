using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(

                new Hotel
                {
                    Id = 1,
                    Name = "Türkiye Hotel",
                    Adress = "İstanbul",
                    Rating = 4.7,
                    CountryId = 1,
                },
                  new Hotel
                  {
                      Id = 2,
                      Name = "İngiltere Hotel",
                      Adress = "İstanbul",
                      Rating = 4.7,
                      CountryId = 2,
                  },
                    new Hotel
                    {
                        Id = 3,
                        Name = "Fıransa Hotel",
                        Adress = "İstanbul",
                        Rating = 4.2,
                        CountryId = 3,
                    },
                      new Hotel
                      {
                          Id = 4,
                          Name = "Bahama Hotel",
                          Adress = "İstanbul",
                          Rating = 3.7,
                          CountryId = 4,
                      },
                        new Hotel
                        {
                            Id = 5,
                            Name = "Türkiye Hotel",
                            Adress = "İstanbul",
                            Rating = 5.0,
                            CountryId = 1,
                        }


                );
        }
    }
}
