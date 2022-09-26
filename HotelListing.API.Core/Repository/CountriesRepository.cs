using AutoMapper;

using  HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Core.Abstract;
using HotelListing.API.Core.Models.Country;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Core.Exceptions;

namespace  HotelListing.API.Core.Repository
{
    public class CountriesRepository:GenericRepository<Country>,ICountriesRepository
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public CountriesRepository(HotelListingDbContext context,IMapper mapper):base(context,mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<CountryDTO> GetDetails(int id)
        {
            var country = await _context.Countries.Include(q => q.Hotels)
                  .ProjectTo<CountryDTO>(_mapper.ConfigurationProvider)
                  .FirstOrDefaultAsync(q=>q.Id==id);
            if (country == null)
                throw new NotFoundException(nameof(GetDetails), id);

            return country;
        }
    }
}
