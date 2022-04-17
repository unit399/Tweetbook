using Refit;
using System;
using Tweetbook.Contracts.V1.Requests;

namespace Tweetbook.Sdk.Sample // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cachedToken = string.Empty;

            var identityApi = RestService.For<IIdentityApi>("https://localhost:7205");
            var tweetbookApi = RestService.For<ITweetbookApi>("https://localhost:7205", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
            });

            var registerResponse = await identityApi.RegisterAsync(new UserRegistrationRequest
            {
                Email = "test14@example.com",
                Password = "Okan123!"
            });

            var loginResponse = await identityApi.LoginAsync(new UserLoginRequest
            {
                Email = "test14@example.com",
                Password = "Okan123!"
            });

            cachedToken = loginResponse.Content.Token;

            var allPosts = await tweetbookApi.GetAllAsync();

            var createdPost = await tweetbookApi.CreateAsync(new CreatePostRequest
            {
                Name = "This is created by SDK",
                Tags = new[] { "sdk" }
            });

            var retrievedPost = await tweetbookApi.GetAsync(createdPost.Content.Id);
            var updatedPost = await tweetbookApi.UpdateAsync(createdPost.Content.Id, new UpdatePostRequest
            {
                Name = "This is updated by the SDK"
            });

            var deletedPost = await tweetbookApi.DeleteAsync(createdPost.Content.Id);
        }
    }
}