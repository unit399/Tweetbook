using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contract.V1;
using Tweetbook.Contract.V1.Requests;
using Tweetbook.Controllers.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : Controller
    {
        private readonly IDataService<Tag, string> tagService;

        public TagsController(IDataService<Tag, string> tagService)
        {
            this.tagService = tagService;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var tags = await this.tagService.GetAllAsync();

            return Ok(tags.Select(tag => new TagResponse
            {
                Name = tag.Name,
                CreatorId = tag.CreatorId,
                CreatedOn = tag.CreatedOn
            }));
        }

        [HttpPost(ApiRoutes.Tags.Create)]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            var tag = new Tag
            {
                Name = request.Name,
                CreatorId = HttpContext.User.Identity.Name,
                CreatedOn = DateTime.UtcNow
            };

            var created = await tagService.CreateTagAsync(tag);

            if (!created)
            {
                return BadRequest(new {error = "Unable to create tag"});
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", tag.Name);

            return Created(locationUri, new TagResponse { Name = tag.Name, CreatorId = tag.CreatorId, CreatedOn = tag.CreatedOn });
        }
        
        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute]string tagName)
        {
            var tag = await this.tagService.GetAsync(tagName);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }
                
        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Policy = "MustWorkForOkan")]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            if (!await this.tagService.DeleteAsync(tagName))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}