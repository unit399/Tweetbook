using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IPostService : IDataService<Post, Guid>
    {
        public Task<bool> UserOwnsPostAsync(string userId, Guid postId);
    }
}