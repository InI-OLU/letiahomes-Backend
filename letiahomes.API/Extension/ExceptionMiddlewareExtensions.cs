using letiahomes.Application.Common;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;
using System.Text.Json;

namespace letiahomes.API.Extension
{
    public static class ExceptionMiddlewareExtensions
    {
        internal static void UseGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async ctx =>
                {
                    ctx.Response.ContentType = "application/json";
                    var ctxFeature = ctx.Features.Get<IExceptionHandlerFeature>();
                    if (ctxFeature != null)
                    {
                        Log.Error("An error occured: {Error}", ctxFeature.Error);
                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await ctx.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = ctx.Response.StatusCode,
                            Message = "An unexpected error occurred" 
                        }.ToString());
                    }
                });
            });
        }
    }
}
