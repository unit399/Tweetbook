using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await this._dataContext.Posts.Include(post => post.Tags).ToListAsync();
        }

        public async Task<Post> GetAsync(Guid Id)
        {
            return await this._dataContext.Posts
                .Include(post => post.Tags)
                .SingleOrDefaultAsync(post => post.Id == Id);
        }

        public async Task<bool> CreateTagAsync(Post post)
        {
            post.Tags?.ForEach(postTag => postTag.TagName = postTag.TagName.ToLower());

            await AddNewTagsAsync(post);
            _dataContext.Posts.Add(post);
            var numCreated = await _dataContext.SaveChangesAsync();

            return numCreated > 0;
        }

        public async Task<bool> UpdateAsync(Post updatedPost)
        {
            updatedPost.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());

            await AddNewTagsAsync(updatedPost);
            _dataContext.Posts.Update(updatedPost);
            var numUpdated = await _dataContext.SaveChangesAsync();

            return numUpdated > 0;
        }

        public async Task<bool> DeleteAsync(Guid postId)
        {
            var postToDelete = await GetAsync(postId);

            if (postToDelete == null)
            {
                return false;
            }

            _dataContext.Posts.Remove(postToDelete);
            var numDeleted = await _dataContext.SaveChangesAsync();

            return numDeleted > 0;
        }

        public async Task<bool> UserOwnsPostAsync(string userId, Guid postId)
        {
            var foundPost = await _dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(post => post.Id == postId);

            if (foundPost == null)
            {
                return false;
            }

            return foundPost.UserId == userId;
        }

        private async Task AddNewTagsAsync(Post post)
        {
            foreach (var newTag in post.Tags)
            {
                var matchingTag = await _dataContext.Tags.SingleOrDefaultAsync(existingTag => existingTag.Name == newTag.TagName);

                if (matchingTag != null)
                {
                    continue;
                }

                _dataContext.Tags.Add(new Tag
                {
                    Name = newTag.TagName,
                    CreatorId = post.UserId,
                    CreatedOn = DateTime.UtcNow
                });
            }
        }
    }
}