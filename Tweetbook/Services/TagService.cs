using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class TagService : IDataService<Tag, string>
    {
        private readonly DataContext _dataContext;

        public TagService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync(PaginationFilter paginationFilter = null)
        {
            if (paginationFilter == null)
                return await _dataContext.Tags.AsNoTracking().ToListAsync();

            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
            return await _dataContext.Tags.AsNoTracking().Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
        }

        public async Task<bool> DeleteAsync(string tagName)
        {
            var tagToDelete = await _dataContext.Tags.FirstOrDefaultAsync(tag => tag.Name == tagName);

            if (tagToDelete == null)
            {
                return false;
            }

            _dataContext.Tags.Remove(tagToDelete);
            var numDeleted = await _dataContext.SaveChangesAsync();

            return numDeleted > 0;
        }

        public async Task<Tag> GetAsync(string tagName)
        {
            return await _dataContext.Tags.SingleOrDefaultAsync(tag => tag.Name == tagName);
        }

        public async Task<bool> CreatePostAsync(Tag newTag)
        {
            _dataContext.Tags.Add(newTag);
            var numCreated = await _dataContext.SaveChangesAsync();

            return numCreated > 0;
        }

        public Task<bool> UpdateAsync(Tag item)
        {
            throw new NotImplementedException();
        }
    }
}