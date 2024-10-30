// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Models.Foundations.ReIdentifications.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Services.Foundations.ReIdentifications
{
    internal partial class ReIdentificationService
    {
        private delegate ValueTask<ReIdentificationRequest> ReturningReIdentificationRequestFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<ReIdentificationRequest> TryCatch(
            ReturningReIdentificationRequestFunction returningReIdentificationRequestFunction)
        {
            try
            {
                return await returningReIdentificationRequestFunction();
            }
            catch (NullReIdentificationRequestException nullIdentificationRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullIdentificationRequestException);
            }
            catch (InvalidReIdentificationRequestException invalidIdentificationRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidIdentificationRequestException);
            }
            catch (AggregateException aggregateException)
            {
                var failedServiceIdentificationRequestException =
                    new FailedServiceReIdentificationException(
                        message: "Failed re-identification aggregate service error occurred, please contact support.",
                        innerException: aggregateException,
                        data: aggregateException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServiceIdentificationRequestException);
            }
            catch (Exception exception)
            {
                var failedServiceIdentificationRequestException =
                    new FailedServiceReIdentificationException(
                        message: "Failed re-identification service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServiceIdentificationRequestException);
            }
        }

        private async ValueTask<ReIdentificationRequestValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var accessAuditValidationException = new ReIdentificationRequestValidationException(
                message: "Re-identification validation error occurred, please fix errors and try again.",
                innerException: exception);

            return accessAuditValidationException;
        }

        private async ValueTask<ReIdentificationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var reIdentificationDependencyValidationException = new ReIdentificationDependencyValidationException(
                message: "Re-identification dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            return reIdentificationDependencyValidationException;
        }

        private async ValueTask<ReIdentificationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var reIdentificationDependencyException = new ReIdentificationDependencyException(
                message: "Re-identification dependency error occurred, contact support.",
                innerException: exception);

            return reIdentificationDependencyException;
        }

        private async ValueTask<ReIdentificationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var reIdentificationServiceException = new ReIdentificationServiceException(
                message: "Re-identification service error occurred, please contact support.",
                innerException: exception);

            return reIdentificationServiceException;
        }
    }
}
