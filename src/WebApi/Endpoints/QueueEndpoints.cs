namespace Dinex.Api;

public static class QueueEndpoints
{
    public static void MapQueueEndpoints(this WebApplication app)
    {
        app.MapPost("/queue/request-to-process-queue", async (
            [FromServices] IQueueAppService queueAppService,
            HttpContext context
            ) =>
        {
            var userId = Base.GetUserId(context);
            var result = await queueAppService.RequestToProcessQueue(userId);
            return Base.GetResult(result);
        })
        .WithName("RequestToProcessQueue")
        .WithOpenApi()
        .Produces<string>(statusCode: 200)
        .Produces<List<string>>(statusCode: 400)
        .RequireAuthorization();
    }
}
