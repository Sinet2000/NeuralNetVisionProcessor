using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetVisionProc.Common.Exceptions;

namespace NetVisionProc.Api.Infra.Filters
{
    public class ApiGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ApiGlobalExceptionFilter> _logger;

        public ApiGlobalExceptionFilter(IWebHostEnvironment env, ILogger<ApiGlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            string path = context.HttpContext.Request.Path;

            switch (context.Exception)
            {
                case DomainOperationException ex:
                    HandleDomainException(context, ex, path);
                    break;
                case ArgumentException argEx:
                    HandleArgumentException(context, argEx, path);
                    break;
                case NotImplementedException notImplEx:
                    HandleNotImplementedException(context, notImplEx);
                    break;
                default:
                    HandleUnhandledException(context);
                    break;
            }

            context.ExceptionHandled = true;
        }

        private static void HandleNotImplementedException(ExceptionContext context, NotImplementedException notImplEx)
        {
            var result = new ErrorResponse(notImplEx.Message);

            context.Result = new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status501NotImplemented
            };
        }

        private static Exception GetRootException(Exception ex)
        {
            if (ex.InnerException is null)
            {
                return ex;
            }

            return GetRootException(ex.InnerException);
        }

        private void HandleUnhandledException(ExceptionContext context)
        {
            var rootEx = GetRootException(context.Exception);
            _logger.LogError(context.Exception, "Error.Unhandled: {Message}", rootEx.Message);

            var result = new ErrorResponse("An unhandled error occured.");
            if (!_env.IsProduction())
            {
                result.Details = rootEx.Message;
            }

            context.Result = new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        private void HandleArgumentException(ExceptionContext context, ArgumentException argEx, string path)
        {
            string errorContext = argEx.ParamName ?? nameof(ArgumentException);
            context.Result = BuildValidationResult(path, errorContext, argEx.Message);
        }

        private void HandleDomainException(ExceptionContext context, DomainOperationException ex, string path)
        {
            if (ex is EntityNotFoundException)
            {
                _logger.LogWarning("Error.{Context}: {Message}", ex.Context, ex.Message);
                context.Result = BuildValidationResult(path, ex.Context, ex.Message, logWarning: false);
            }
            else
            {
                context.Result = BuildValidationResult(path, ex.Context, ex.Message);
            }
        }

        private IActionResult BuildValidationResult(string path, string errorContext, string errorMessage, bool logWarning = true)
        {
            if (logWarning)
            {
                _logger.LogWarning("Error.{Context}: {Message}", errorContext, errorMessage);
            }

            var problemDetails = new ValidationProblemDetails()
            {
                Instance = path,
                Status = StatusCodes.Status400BadRequest
            };

            problemDetails.Errors.Add(errorContext, [errorMessage]);

            return new BadRequestObjectResult(problemDetails);
        }

        private sealed record ErrorResponse
        {
            public ErrorResponse(string message)
            {
                Message = message;
            }

            public string Message { get; init; } = null!;

            public string? Details { get; set; }
        }
    }
}