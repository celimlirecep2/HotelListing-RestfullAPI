using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using AutoMapper;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.OData.Query;

using HotelListing.API.Data;
using HotelListing.API.Core.Abstract;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Exceptions;

namespace HotelListing.API.Controllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CountriesV2Controller : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countries;
        private readonly ILogger<CountriesV2Controller> _logger;

        public CountriesV2Controller( IMapper mapper,ICountriesRepository countries,ILogger<CountriesV2Controller> logger)
        {
            
            _mapper = mapper;
            this._countries = countries;
            this._logger = logger;
        }

        // GET: api/Countries
        //https://localhost:7234/api/v2/Countries?$select=shortName,name
        //$filter=name eq 'Türkiye'
        //?$filter=name eq 'Türkiye'&$orderby=name
        [HttpGet]
        [Authorize]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDTO>>> GetCountries()
        {
            var countries = await _countries.GetAllAsync();
            var records=_mapper.Map<List<GetCountryDTO>>(countries);
       
            return records ;
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CountryDTO>> GetCountry(int id)
        {
            var country =await _countries.GetDetails(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);
            }
            var countryDto=_mapper.Map<CountryDTO>(country);

            return Ok(countryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDTO updateCountryDTO)
        {

            if (id != updateCountryDTO.Id)
            {
                return BadRequest();
            }

            var country = await _countries.GetAsync(updateCountryDTO.Id);
            if (country==null)
            {
                throw new NotFoundException(nameof(PutCountry), id);
            }

            _mapper.Map(updateCountryDTO, country);
            try
            {
                await _countries.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    throw new NotFoundException(nameof(PutCountry), id);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDTO countryDTO)
        {
            var country=_mapper.Map<Country>(countryDTO);
           await _countries.AddAsync(country);
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")] //[Authorize(Roles = "Administrator,User")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countries.GetAsync(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(DeleteCountry), id);
            }

           await _countries.DeleteAsync(id);
            

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countries.Exist(id);
        }
    }
}
