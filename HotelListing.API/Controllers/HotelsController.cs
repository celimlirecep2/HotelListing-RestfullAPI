using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;

using HotelListing.API.Data;
using HotelListing.API.Core.Abstract;
using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Core.Models;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHotelRepository _hotel;

        public HotelsController(IMapper mapper,IHotelRepository hotel)
        {
            this._mapper = mapper;
            this._hotel = hotel;
        }

        // GET: api/Hotels
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetHotels()
        {
            var hotel= await _hotel.GetAllAsync<HotelDTO>();
             return Ok(hotel);
        }
        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<PagesResult<HotelDTO>>> GetPagesHotels([FromQuery] QueryParameters queryParameters)
        {
            var pagesHotelResult = await _hotel.GetAllAsync<HotelDTO>(queryParameters);
            return Ok(pagesHotelResult);
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDTO>> GetHotel(int id)
        {
            var hotel = await _hotel.GetAsync<HotelDTO>(id);

            if (hotel == null)
            {
                return NotFound();
            }

            return Ok(hotel);
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, UpdateHotelDTO hotelDTO)
        {
            if (id != hotelDTO.Id)
            {
                return BadRequest();
            }

            var hotel= await _hotel.GetAsync(id);
            if (hotel==null)
            {
                return NotFound();
            }
            _mapper.Map(hotelDTO,hotel);
            try
            {
                await _hotel.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HotelDTO>> PostHotel(CreateHotelDTO hotelDTO)
        {
           
            var hotel= await _hotel.AddAsync<CreateHotelDTO, HotelDTO>(hotelDTO);
            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _hotel.GetAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            await _hotel.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotel.Exist(id);
        }
    }
}
