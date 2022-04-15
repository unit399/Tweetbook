namespace Tweetbook.Services
{
    public interface IDataService<TItem, TKey>
    {
        Task<IEnumerable<TItem>> GetAllAsync();

        Task<TItem> GetAsync(TKey key);

        Task<bool> CreateTagAsync(TItem item);

        Task<bool> UpdateAsync(TItem item);

        Task<bool> DeleteAsync(TKey key);
    }
}