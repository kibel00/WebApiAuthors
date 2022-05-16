namespace WebApiAuthors.Middlewares
{

    public static class HttpLogAnswerMiddlewareExtention
    {
        public static IApplicationBuilder UseHttpLogAnswer(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpLogAnswerMiddleware>();
        }
    }
    public class HttpLogAnswerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public HttpLogAnswerMiddleware(RequestDelegate next, ILogger<HttpLogAnswerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalResponseBody = context.Response.Body;
                context.Response.Body = memoryStream;

                await next(context);

                memoryStream.Seek(0, SeekOrigin.Begin);
                string answer = new StreamReader(memoryStream).ReadToEnd();
                memoryStream.Seek(0, SeekOrigin.Begin);

                await memoryStream.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;

                logger.LogInformation(answer);
            }
        }
    }
}
