using HotelListing.API.Core.Models.Country;
using  HotelListing.API.Data;

namespace  HotelListing.API.Core.Abstract
{
    public interface ICountriesRepository: IGenericRepository<Country>
    {
        Task<CountryDTO> GetDetails(int id);
    }
}
