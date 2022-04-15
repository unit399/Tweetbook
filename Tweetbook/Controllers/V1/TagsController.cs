using AutoMapper;
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
        private readonly IDataService<Tag, string> _tagService;
        private readonly IMapper _mapper;

        public TagsController(IDataService<Tag, string> tagService, IMapper mapper)
        {
            _tagService = tagService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _tagService.GetAllAsync();

            return Ok(_mapper.Map<List<TagResponse>>(tags));
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

            var created = await _tagService.CreateTagAsync(tag);

            if (!created)
            {
                return BadRequest(new {error = "Unable to create tag"});
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", tag.Name);

            return Created(locationUri, _mapper.Map<TagResponse>(tag));
        }
        
        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute]string tagName)
        {
            var tag = await _tagService.GetAsync(tagName);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TagResponse>(tag));
        }
                
        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Policy = "MustWorkForOkan")]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            if (!await _tagService.DeleteAsync(tagName))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}