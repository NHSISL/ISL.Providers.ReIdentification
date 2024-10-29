// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions
{
    public partial class ReIdentificationAbstractionProvider
    {
        private delegate ValueTask<ReIdentificationRequest> ReturningReIdentificationRequestFunction();

        private async ValueTask<ReIdentificationRequest> TryCatch(
            ReturningReIdentificationRequestFunction returningReIdentificationRequestFunction)
        {
            try
            {
                return await returningReIdentificationRequestFunction();
            }
            catch (Xeption ex) when (ex is IReIdentificationProviderValidationException)
            {
                throw CreateValidationException(ex);
            }
            catch (Xeption ex) when (ex is IReIdentificationProviderDependencyValidationException)
            {
                throw CreateValidationException(ex);
            }
            catch (Xeption ex) when (ex is IReIdentificationProviderDependencyException)
            {
                throw CreateDependencyException(ex);
            }
            catch (Xeption ex) when (ex is IReIdentificationProviderServiceException)
            {
                throw CreateServiceException(ex);
            }
            catch (Exception ex)
            {
                var uncatagorizedNotificationProviderException =
                    new UncatagorizedReIdentificationProviderException(
                        message: "Re-identification provider not properly implemented. Uncatagorized errors found, " +
                            "contact the notification provider owner for support.",
                        innerException: ex,
                        data: ex.Data);

                throw CreateUncatagorizedServiceException(uncatagorizedNotificationProviderException);
            }
        }

        private ReIdentificationProviderValidationException CreateValidationException(
            Xeption exception)
        {
            var notificationValidationProviderException =
                new ReIdentificationProviderValidationException(
                    message: "Re-identification validation errors occurred, please try again.",
                    innerException: exception,
                    data: exception.Data);

            return notificationValidationProviderException;
        }

        private ReIdentificationProviderDependencyException CreateDependencyException(
            Xeption exception)
        {
            var notificationDependencyProviderException = new ReIdentificationProviderDependencyException(
                message: "Re-identification dependency error occurred, contact support.",
                innerException: exception,
                data: exception.Data);

            return notificationDependencyProviderException;
        }

        private ReIdentificationProviderServiceException CreateServiceException(
            Xeption exception)
        {
            var notificationServiceProviderException = new ReIdentificationProviderServiceException(
                message: "Re-identification service error occurred, contact support.",
                innerException: exception,
                data: exception.Data);

            return notificationServiceProviderException;
        }

        private ReIdentificationProviderServiceException CreateUncatagorizedServiceException(
            Exception exception)
        {
            var notificationServiceProviderException = new ReIdentificationProviderServiceException(
                message: "Uncatagorized re-identification service error occurred, contact support.",
                innerException: exception as Xeption,
                data: exception.Data);

            return notificationServiceProviderException;
        }
    }
}
