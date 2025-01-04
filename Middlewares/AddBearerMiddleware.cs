namespace PicShelfServer.Middlewares;

public class AddBearerMiddleware
{
    private readonly RequestDelegate _next;

    public AddBearerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            // Check if the header starts with "Bearer", if not, prepend "Bearer "
            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.Headers["Authorization"] = $"Bearer {authHeader}";
            }
        }

        await _next(context);
    }
}
