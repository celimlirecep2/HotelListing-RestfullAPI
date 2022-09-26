using AutoMapper;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Core.Models.Users;
using  HotelListing.API.Data;


namespace  HotelListing.API.Core.Configuration
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            #region Country Mapping
            CreateMap<Country,CreateCountryDTO>().ReverseMap();
            CreateMap<Country,GetCountryDTO>().ReverseMap();
            CreateMap<Country,CountryDTO>().ReverseMap();
            CreateMap<Country,UpdateCountryDTO>().ReverseMap();
            #endregion

            #region Hotel Mapping
            CreateMap<Hotel, HotelDTO>().ReverseMap();
            #endregion

            #region User Mapping
            CreateMap<ApiUser, ApiUserDTO>().ReverseMap();
            #endregion
        }
    }
}
