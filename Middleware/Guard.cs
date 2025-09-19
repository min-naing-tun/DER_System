using Microsoft.IdentityModel.Tokens;
using DER_System.Helper;
using DER_System.Utilities;
using System.Text;
using Newtonsoft.Json;

namespace NKC_Resource_Allocation.Middleware
{
    public class Guard
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private bool _hasRun = false;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Guard(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            HttpRequest request = context.Request;

            bool apiRequestFlag = request.Path.Value!.Contains("api");

            if (apiRequestFlag)
            {
                string requestAPIKey = request.Headers["APIKey"].ToString();

                if (!requestAPIKey.IsNullOrEmpty())
                {
                    if(requestAPIKey != new KeyConfig().GetApiKey())
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("Unauthorized access detected! \n( MBL DER API Server )");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Unauthorized access detected! \n( MBL DER API Server )");
                    return;
                }
            }
            await _next(context);
        }
    }
}
