using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Cache;
using Tweetbook.Contract.V1;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Requests.Queries;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Controllers.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Helpers;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostsQuery query, [FromQuery]PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var postQueryFilter = _mapper.Map<GetAllPostsFilter>(query);
            var posts = await _postService.GetAllAsync(postQueryFilter, paginationFilter);
            var postsResponse = _mapper.Map<List<PostResponse>>(posts);

            if (paginationFilter == null || paginationFilter.PageNumber < 1 || paginationFilter.PageSize < 1)
                return Ok(new PagedResponse<PostResponse>(postsResponse));

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, paginationFilter, postsResponse);

            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest postRequest)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(HttpContext.GetUserId(), postId);

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You do not own this post" });
            }

            var post = await _postService.GetAsync(postId);
            post.Name = postRequest.Name;
            post.Tags = postRequest.Tags.Select(tagName => new PostTag { TagName = tagName, PostId = post.Id }).ToList();

            if (!await _postService.UpdateAsync(post))
            {
                return NotFound();
            }

            return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest) 
        {           
            var newPostId = Guid.NewGuid();
            var post = new Post
            {
                Id = newPostId,
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(tagName => new PostTag { TagName = tagName, PostId = newPostId }).ToList()
            };

            await _postService.CreatePostAsync(post);

            var locationUri = _uriService.GetPostUri(post.Id.ToString());           

            return Created(locationUri, new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(HttpContext.GetUserId(), postId);

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You do not own this post" });
            }

            if (!await _postService.DeleteAsync(postId))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}