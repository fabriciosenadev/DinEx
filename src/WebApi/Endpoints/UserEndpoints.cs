namespace Dinex.Api;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        #region Anonymous endpoints
        app.MapPost("/user", async (
                [FromServices] IUserAppService userAppService,
                HttpContext context,
                CreateUserRequest request) =>
            {
                var result = await userAppService.CreateUserAsync(request);
                return Base.GetResult(result);
            })
        .WithName("CreateNewUser")
        .Produces<string>(statusCode: 200)
        .Produces<List<string>>(statusCode: 400)
        .WithOpenApi();

        app.MapPost("/user/request-activation-link", async (
            [FromServices] IUserAppService userAppService,
            HttpContext context,
            ActivationUrlRequest request) =>
        {
            var result = await userAppService.RequestActivationLinkAsync(request);
            return Base.GetResult(result);
        })
        .WithName("RequestUserActivationLink")
        .Produces<string>(statusCode: 200)
        .Produces<string>(statusCode: 404)
        .WithOpenApi();

        app.MapPost("/user/activate", async (
            [FromServices] IUserAppService userAppService,
            HttpContext context,
            ActivationRequest request) => 
        {
            var result = await userAppService.ActivateUser(request);
            return Base.GetResult(result);
        })
        .WithName("ActivateUser")
        .Produces<string>(statusCode: 200)
        .Produces<List<string>>(statusCode: 400)
        .Produces<string>(statusCode: 404)
        .WithOpenApi();

        app.MapPost("/user/login", async (
            [FromServices] IUserAppService userAppService,
            HttpContext context,
            AuthenticationRequest request
            ) =>
        {
            var result = await userAppService.AuthenticateAsync(request);
            return Base.GetResult(result);
        })
        .WithName("UserLogin")
        .Produces<string>(statusCode: 200)
        .Produces<List<string>>(statusCode: 400)
        .Produces<string>(statusCode: 404)
        .WithOpenApi();
        #endregion

        #region Authenticated endpoints
        app.MapGet("/user", async (
            [FromServices] IUserAppService userAppService,
            HttpContext context) =>
        {
            var userId = Base.GetUserId(context);
            var result = await userAppService.GetByIdAsync(userId);
            return Base.GetResult(result);
        })
        .WithName("GetUserById")
        .Produces<string>(statusCode: 200)
        .Produces<string>(statusCode: 404)
        .WithOpenApi()
        .RequireAuthorization();

        app.MapPut("/user", () => { })
        .WithName("UpdateUser")
        .WithOpenApi()
        .RequireAuthorization();
        #endregion
    }
}
