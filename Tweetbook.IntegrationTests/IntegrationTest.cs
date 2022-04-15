using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tweetbook.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using System;
using Tweetbook.Controllers.V1.Responses;
using Tweetbook.Contract.V1;
using System.Net.Http.Formatting;
using System.Collections.Generic;

namespace Tweetbook.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient TestClient;
        private readonly IServiceProvider _serviceProvider;
        
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>()
               .WithWebHostBuilder(builder =>
               {
                   builder.ConfigureServices(services =>
                   {
                       var descriptor = services.SingleOrDefault(d => d.ServiceType ==
                                typeof(DbContextOptions<DataContext>));

                       if (descriptor != null)
                           services.Remove(descriptor);                       
                       
                       services.AddDbContext<DataContext>(options => { options.UseInMemoryDatabase("TestDb"); });
                       services.BuildServiceProvider();
                   });
               });

            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@test8.com",
                Password = "Example123!"
            });
            
            List<MediaTypeFormatter> formatters = new List<MediaTypeFormatter>();
            formatters.Add(new JsonMediaTypeFormatter());
            

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>(formatters);
            return registrationResponse.Token;
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            return await response.Content.ReadAsAsync<PostResponse>();
        }

        public void Dispose()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();
        }
    }
}
