using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SliceWebApp.API.Contracts;
using SliceWebApp.API.Database;
using SliceWebApp.API.Shared;

namespace SliceWebApp.API.Features.Posts;

public static class GetPost
{
    public class Query : IRequest<Result<PostResponse>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<PostResponse>>
    {
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<PostResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var postResponse = await _dbContext
                .Posts
                .AsNoTracking()
                .Where(article => article.Id == request.Id)
                .Select(article => new PostResponse
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    CreatedOnUtc = article.CreatedOnUtc
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (postResponse is null)
            {
                return Result.Failure<PostResponse>(new Error(
                    "GetArticle.Null",
                    "The article with the specified ID was not found"));
            }

            return postResponse;
        }
    }
}

public class GetArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetPost.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}