using FastEndpoints;
using System.Text.Json.Serialization;
using FluentValidation;

namespace FluentValidationIssue.Feature;

[
    JsonPolymorphic(TypeDiscriminatorPropertyName = "_t"),
    JsonDerivedType(typeof(MultiChoiceQuestionRequest), "mcq"),
    JsonDerivedType(typeof(RatingQuestionRequest), "rq")
]
public class BaseQuestionRequest
{
    public int Id { get; set; }
}

public class BaseQuestionRequestValidator : Validator<BaseQuestionRequest>
{
    public BaseQuestionRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
public class MultiChoiceQuestionRequest : BaseQuestionRequest
{
    public string Question { get; set; }
}

public class MultiChoiceQuestionRequestValidator : Validator<MultiChoiceQuestionRequest>
{
    public MultiChoiceQuestionRequestValidator()
    {
        Include(new BaseQuestionRequestValidator());
        RuleFor(r => r.Question).NotEmpty();
    }
}
public class RatingQuestionRequest : BaseQuestionRequest
{
    public int Rating { get; set; }
}

public class RatingQuestionRequestValidator : Validator<RatingQuestionRequest>
{
    public RatingQuestionRequestValidator()
    {
        Include(new BaseQuestionRequestValidator());
        RuleFor(r => r.Rating).GreaterThan(1);
    }
}
public class Request
{
    public List<BaseQuestionRequest> Questions { get; set; } = [];
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleForEach(x => x.Questions).SetInheritanceValidator(
            v =>
            {
                v.Add(new MultiChoiceQuestionRequestValidator());
                v.Add(new RatingQuestionRequestValidator());
            });
    }
}
sealed class MyEndpoint : Endpoint<Request>
{
    public override void Configure()
    {
        Post("test");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        await SendAsync(r);
    }
}