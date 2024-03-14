namespace Dinex.Api;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this WebApplication app)
    {
        app.MapPost("/file/investing-statement",
            [Consumes("multipart/form-data")] async (
            [FromServices] IFileAppService fileAppService,
            HttpContext context,
            [FromForm] HistoryFileRequest request
            ) =>
        {
            var userId = Base.GetUserId(context);
            var result = await fileAppService.UploadInvestingStatement(request, userId);
            return Base.GetResult(result);
        })
        .WithName("UploadInvestingStatement")
        .WithOpenApi()
        .Produces<string>(statusCode: 200)
        .Produces<List<string>>(statusCode: 400)
        .RequireAuthorization()
        .DisableAntiforgery();
    }
}
