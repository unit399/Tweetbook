using Swashbuckle.AspNetCore.Filters;
using Tweetbook.Controllers.V1.Responses;

namespace Tweetbook.SwaggerExamples.Responses
{
    public class TagResponseExample : IExamplesProvider<TagResponse>
    {
        public TagResponse GetExamples()
        {
            return new TagResponse
            {
                Name = "new tag"
            };
        }
    }
}
