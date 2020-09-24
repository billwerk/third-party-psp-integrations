using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Web.Middlewares
{
    public class LogContextValuesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public LogContextValuesMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task Invoke(HttpContext context)
        {
            if (!_httpContextAccessor.HttpContext.Items.TryGetValue("UserId", out var userId))
            {
                return _next.Invoke(context);
            }
            
            using (LogContext.PushProperty("UserId", userId))
            {
                return _next.Invoke(context);
            }

        }
    }
}