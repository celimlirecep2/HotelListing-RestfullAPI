namespace  HotelListing.API.Core.Models.Hotel
{
    public class HotelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public string Adress { get; set; }
        public int CountryId { get; set; }

    }
}
