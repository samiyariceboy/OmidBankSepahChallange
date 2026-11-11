using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Taday.Corelibrary.Application.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Taday.Corelibrary.Application.Middlewares;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Domain.Exceptions;

namespace OmidBankSepah.MasterData.Presentation.Middlewares;

public sealed class MasterDataExceptionHandlerMiddleware(
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
            Logger.LogError(concurrencyException, "Product availability changed concurrently.");
            additionalData = concurrencyException.InnerException;
            apiResultStatusCode = (ApiResultStatusCode)StatusCodes.Status409Conflict;
            message = environment.IsDevelopment()
                ? concurrencyException.Message
                : "Product availability changed concurrently. Retry the request.";
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

        base.HandleException(exception, out message, out additionalData, out apiResultStatusCode);
    }

    protected override string PrepareAppExceptionMessage(AppException exception, bool isDevelopment)
    {
        return base.PrepareAppExceptionMessage(exception, false);
    }
}
