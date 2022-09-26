using AutoMapper;

using  HotelListing.API.Data;
using HotelListing.API.Core.Abstract;

namespace  HotelListing.API.Core.Repository
{
    public class HotelRepository:GenericRepository<Hotel>,IHotelRepository
    {
        public HotelRepository(HotelListingDbContext context,IMapper mapper):base(context,mapper)
        {
            
        }
    }
}
