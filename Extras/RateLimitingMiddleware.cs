namespace BlogPersonal.Extras
{
    public class RateLimitingMiddleware
    {
        private static Dictionary<string, int> _requestCounts = new Dictionary<string, int>();
        private static TimeSpan _timeSpan = TimeSpan.FromMinutes(1);
        private static int _maxRequests = 100;

        public RateLimitingMiddleware()
        {
            
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            if (!_requestCounts.ContainsKey(clientIp))
            {
                _requestCounts[clientIp] = 0;
            }

            _requestCounts[clientIp]++;

            if (_requestCounts[clientIp] > _maxRequests)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }

            await next(context);

            Task.Delay(_timeSpan).ContinueWith(_ => _requestCounts[clientIp]--);
        }
    }
}
