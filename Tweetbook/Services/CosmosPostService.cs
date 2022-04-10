using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class CosmosPostService : IPostService
    {
        public Task<bool> CreatePostAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePostAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPostByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetPostsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
