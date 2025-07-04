using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace HotelABP
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled exception occurred.");

            var result = new ObjectResult(new
            {
                Success = false,
                Message = "服务器发生错误，请联系管理员。",
                Detail = context.Exception.Message
            })
            {
                StatusCode = 500
            };
            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
} 