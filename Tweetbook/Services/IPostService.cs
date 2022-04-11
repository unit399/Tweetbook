﻿using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post> GetPostByIdAsync(Guid id);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> DeletePostAsync(Guid id);
        Task<bool> CreatePostAsync(Post post);
        Task <bool> UserOwnsPostAsync(Guid postId, string v);
    }
}
