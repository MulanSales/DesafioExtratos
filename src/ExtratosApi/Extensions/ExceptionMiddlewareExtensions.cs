using System.Net;
using ExtratosApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace ExtratosApi.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError => {
                appError.Run(async context => {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null) {
                        await context.Response.WriteAsync(new ErrorDetails() {
                            StatusCode = context.Response.StatusCode,
                            Message= "Internal Server Error. We're currently working to fix it."
                        }.ToString());
                    }
                });
            });
        }
    }
}