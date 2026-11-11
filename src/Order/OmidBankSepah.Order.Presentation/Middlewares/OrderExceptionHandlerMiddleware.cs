using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Taday.Corelibrary.Application.Middlewares;

using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Taday.Corelibrary.Application.Middlewares;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Domain.Exceptions;

namespace OmidBankSepah.Order.Presentation.Middlewares;

public sealed class OrderExceptionHandlerMiddleware(
    RequestDelegate next,
    IHostEnvironment environment,
    ILogger<CoreExeptionHandlerMiddleware> logger)
    : CoreExeptionHandlerMiddleware(next, environment, logger)
{
    protected override void HandleException(
        Exception exception,
        out string message,
        out object? additionalData,
        out ApiResultStatusCode apiResultStatusCode)
    {
        if (exception is DbUpdateConcurrencyException concurrencyException)
        {
            Logger.LogError(concurrencyException, "Order changed concurrently.");
            additionalData = concurrencyException.InnerException;
            apiResultStatusCode = (ApiResultStatusCode)StatusCodes.Status409Conflict;
            message = environment.IsDevelopment()
                ? concurrencyException.Message
                : "The order changed concurrently. Retry with the same idempotency key.";
            return;
        }

        if (exception is DbUpdateException databaseException)
        {
            Logger.LogError(databaseException, "Database Error");
            apiResultStatusCode = ApiResultStatusCode.ServerError;
            additionalData = databaseException.InnerException;
            message = environment.IsDevelopment()
                ? databaseException.Message
                : "Database Error";
            return;
        }

        if (exception is RpcException rpcException)
        {
            var statusCode = rpcException.StatusCode switch
            {
                StatusCode.Unavailable or StatusCode.DeadlineExceeded => StatusCodes.Status503ServiceUnavailable,
                StatusCode.Aborted => StatusCodes.Status409Conflict,
                StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                StatusCode.NotFound => StatusCodes.Status404NotFound,
                _ => (int?)null
            };

            if (statusCode is not null)
            {
                Logger.LogError(rpcException, "Product availability gRPC call failed.");
                additionalData = null;
                apiResultStatusCode = (ApiResultStatusCode)statusCode.Value;
                message = rpcException.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded
                    ? "Inventory service is unavailable. Retry with the same idempotency key."
                    : rpcException.Status.Detail;
                return;
            }
        }

        base.HandleException(exception, out message, out additionalData, out apiResultStatusCode);
    }

    protected override string PrepareAppExceptionMessage(AppException exception, bool isDevelopment)
    {
        return base.PrepareAppExceptionMessage(exception, false);
    }
}
