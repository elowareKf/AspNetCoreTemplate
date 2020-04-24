using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Template.Server.Authentication {
    public class ApiTokenFilterMiddleware : IMiddleware {
        private readonly string _apiKey;


        public ApiTokenFilterMiddleware(string apiKey) {
            _apiKey = apiKey;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next) {
            if (context.Request.Path.StartsWithSegments("/swagger"))
                return next.Invoke(context);
            
            if (context.Request.Headers["ApiKey"] != _apiKey) {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsync( "API key is invalid").Wait();
                return Task.CompletedTask;
            }

            return next.Invoke(context);
        }
    }
}