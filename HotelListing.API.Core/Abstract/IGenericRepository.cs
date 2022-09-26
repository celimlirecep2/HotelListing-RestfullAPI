using HotelListing.API.Core.Models;


namespace  HotelListing.API.Core.Abstract
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int? id);
        Task<TResult> GetAsync<TResult>(int? id);
        Task<List<T>> GetAllAsync();
        Task<List<TResult>> GetAllAsync<TResult>();
        Task<PagesResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
        Task<T> AddAsync(T entity);
        Task<TResult> AddAsync<TSource, TResult>(TSource entity);
        Task UpdateAsync(T entity);
        Task UpdateAsync<TSource>(int id,TSource entity);
        Task DeleteAsync(int id);
        Task<bool> Exist(int id);
        

    }
}
