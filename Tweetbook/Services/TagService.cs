using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class TagService : IDataService<Tag, string>
    {
        private readonly DataContext dataContext;

        public TagService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await this.dataContext.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<bool> DeleteAsync(string tagName)
        {
            var tagToDelete = await dataContext.Tags.FirstOrDefaultAsync(tag => tag.Name == tagName);

            if (tagToDelete == null)
            {
                return false;
            }

            this.dataContext.Tags.Remove(tagToDelete);
            var numDeleted = await this.dataContext.SaveChangesAsync();

            return numDeleted > 0;
        }

        public async Task<Tag> GetAsync(string tagName)
        {
            return await dataContext.Tags.SingleOrDefaultAsync(tag => tag.Name == tagName);
        }

        public async Task<bool> CreateTagAsync(Tag newTag)
        {
            this.dataContext.Tags.Add(newTag);
            var numCreated = await dataContext.SaveChangesAsync();

            return numCreated > 0;
        }

        public Task<bool> UpdateAsync(Tag item)
        {
            throw new NotImplementedException();
        }
    }
}