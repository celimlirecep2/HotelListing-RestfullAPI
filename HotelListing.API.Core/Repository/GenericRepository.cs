using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Core.Exceptions;
using System.Diagnostics.Metrics;
using  HotelListing.API.Data;

using Microsoft.EntityFrameworkCore;
using HotelListing.API.Core.Abstract;
using HotelListing.API.Core.Models;

namespace  HotelListing.API.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(HotelListingDbContext context,IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource source)
        {
            var entity = _mapper.Map<T>(source);
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<TResult>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity=await GetAsync(id);
            if (entity is null)
            {
                throw new NotFoundException(typeof(T).Name,id);
            }
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exist(int id)
        {
            var entity=await GetAsync(id);
            return entity!=null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<PagesResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
        {
            var totalSıze= await _context.Set<T>().CountAsync();
            var items=await _context.Set<T>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new PagesResult<TResult>
            {
                Items = items,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSıze
            };
        }

        public async Task<List<TResult>> GetAllAsync<TResult>()
        {
            return await _context.Set<T>()
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id==null)
            {
                return null;
            }
           return await _context.Set<T>().FindAsync(id);
        }

        public async Task<TResult> GetAsync<TResult>(int? id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            if (result is null)
                throw new NotFoundException(typeof(T).Name, id.HasValue? id:"No Key Provided");
            return _mapper.Map<TResult>(result);

            
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<TSource>(int id, TSource source)
        {
            var entity = await GetAsync(id);
            if (entity == null)
                throw new NotFoundException(typeof(T).Name, id);
            _mapper.Map(source, entity);
            _context.Update(entity);
           await _context.SaveChangesAsync();

        }
    }
}
