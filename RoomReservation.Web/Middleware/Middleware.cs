namespace RoomReservation.Web.Middleware
{
	public class ApiTokenMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IConfiguration _configuration;

		public ApiTokenMiddleware(RequestDelegate next, IConfiguration configuration)
		{
			_next = next;
			_configuration = configuration;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.Request.Path.StartsWithSegments("/api"))
			{
				await _next(context);
				return;
			}

			if (context.Request.Path.StartsWithSegments("/api/auth"))
			{
				await _next(context);
				return;
			}

			string? expectedToken = _configuration["ApiSettings:Token"];

			if (string.IsNullOrWhiteSpace(expectedToken))
			{
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				await context.Response.WriteAsync("API token is not configured.");
				return;
			}

			if (!context.Request.Headers.TryGetValue("X-Api-Token", out var providedToken))
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsync("Missing API token.");
				return;
			}

			if (providedToken != expectedToken)
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsync("Invalid API token.");
				return;
			}

			await _next(context);
		}
	}
}