using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tweetbook.Contract.V1;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Controllers.V1.Responses;
using Tweetbook.Domain;
using Xunit;

namespace Tweetbook.IntegrationTests
{
    public class PostControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var posts = (await response.Content.ReadAsAsync<PagedResponse<PostResponse>>());
            posts.Data.Should().BeEmpty();
            posts.Data.Should().BeNull();
        }

        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsInDatabase()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPostResponse = await CreatePostAsync(new CreatePostRequest {Name = "Test post", Tags = new List<string>()});
            var createdPost = createdPostResponse.Data;

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPostResponse = await response.Content.ReadAsAsync<Response<PostResponse>>();
            var returnedPost = returnedPostResponse.Data;
            returnedPost.Id.Should().Be(createdPost.Id);
            returnedPost.Name.Should().Be("Test post");
        }
    }
}
