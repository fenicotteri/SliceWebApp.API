using Carter;
using FluentValidation;
using MediatR;
using SliceWebApp.API.Contracts;
using SliceWebApp.API.Database;
using SliceWebApp.API.Entities;
using SliceWebApp.API.Shared;

namespace SliceWebApp.API.Features.Posts;

public static class CreatePost
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Content).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly DataContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(DataContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(new Error(
                    "CreateArticle.Validation",
                    validationResult.ToString()));
            }

            var article = new Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                CreatedOnUtc = DateTime.UtcNow
            };

            _dbContext.Add(article);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return article.Id;
        }
    }
}

public class CreateArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/articles", async (CreatePostRequest request, ISender sender) =>
        {
            var command = new CreatePost.Command
            {
                Content = request.Content,
                Title = request.Title,
            };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
