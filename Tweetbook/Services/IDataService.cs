using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IDataService<TItem, TKey>
    {
        Task<IEnumerable<TItem>> GetAllAsync(PaginationFilter paginationFilter = null);

        Task<TItem> GetAsync(TKey key);

        Task<bool> CreatePostAsync(TItem item);

        Task<bool> UpdateAsync(TItem item);

        Task<bool> DeleteAsync(TKey key);
    }
}