using DER_System;
using DER_System.Helper;
using DER_System.Model;
using DER_System.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

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
            var originalBodyStream = context.Response.Body;
            HttpRequest request = context.Request;

            bool apiRequestFlag = request.Path.Value!.Contains("api");

            //access authority checking
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
                    else
                    {
                        // <-- valid --> //
                        
                        //scoping db context
                        var dbContext = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<DerDbContext>();
                        API_Logs log = new API_Logs();

                        context.Request.EnableBuffering();

                        //request session
                        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                        {
                            var requestBody = await reader.ReadToEndAsync();
                            context.Request.Body.Position = 0; // reset

                            //parameters session
                            var parameters = new Dictionary<string, object?>();
                            foreach(var kv in request.Query)
                            {
                                parameters[kv.Key] = kv.Value.ToString();
                            }
                            var json = JsonConvert.SerializeObject(parameters, Formatting.Indented);

                            log.Type = "Request";
                            log.Method_Or_Status_Code = request.Method;
                            log.Endpoint = request.Path.Value;
                            log.Parameters = json;
                            log.Data = requestBody;
                            log.ActionTime = DateTime.Now;
                            await dbContext.API_Logs.AddAsync(log);
                            await dbContext.SaveChangesAsync();
                        }

                        //response session
                        using (var newBodyStream = new MemoryStream())
                        {
                            context.Response.Body = newBodyStream;
                            await _next(context);

                            // Copy response body to a string
                            newBodyStream.Seek(0, SeekOrigin.Begin);
                            var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();
                            HttpResponse httpResponse = context.Response;

                            log = new API_Logs();
                            log.Type = "Response";
                            log.Method_Or_Status_Code = httpResponse.StatusCode.ToString();
                            log.Endpoint = request.Path.Value;
                            log.Parameters = "";
                            log.Data = responseBody;
                            log.ActionTime = DateTime.Now;
                            await dbContext.API_Logs.AddAsync(log);
                            await dbContext.SaveChangesAsync();

                            // Copy the newBodyStream back to the original body stream
                            newBodyStream.Seek(0, SeekOrigin.Begin);
                            await newBodyStream.CopyToAsync(originalBodyStream);
                        }
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
            else
            {
                await _next(context);
            }
        }
    }
}
