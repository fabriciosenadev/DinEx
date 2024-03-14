namespace Dinex.Api
{
    public class Base
    {
        public static IResult GetResult<T>(OperationResult<T> result)
        {
            if (result.HasErrors())
            {
                var errors = new { result.Errors };
                if (result.IsNotFound)
                {
                    return Results.NotFound(errors);
                }
                else if (!result.IsValid && !result.IsNotFound && !result.InternalServerError)
                {
                    return Results.BadRequest(errors);
                }
                else if(result.InternalServerError)
                {
                    var internalError = result.Errors.ToList()[0];
                    return Results.Problem(internalError, statusCode: StatusCodes.Status500InternalServerError);
                }
            }

            var data = new { result.Data };
            return Results.Ok(result.Data);
        }

        public static IResult GetResult(OperationResult result)
        {
            if(result.HasErrors())
            {
                var errors = new { result.Errors };
                if (result.IsNotFound)
                {
                    return Results.NotFound(errors);
                }
                else if (!result.IsValid && !result.IsNotFound && !result.InternalServerError)
                {
                    return Results.BadRequest(errors);
                }
                else if (result.InternalServerError)
                {
                    var internalError = result.Errors.ToList()[0];
                    return Results.Problem(internalError, statusCode: StatusCodes.Status500InternalServerError);
                }
            }

            return Results.Ok();
        }

        public static Guid GetUserId(HttpContext context)
        {
            if(context.Items["UserId"] is Guid userId)
            {
                return userId;
            }
            else
            {
                return Guid.Empty;
            }
        }
    }
}
