// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Brokers.OfflineSources;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Models.Foundations.ReIdentifications.Exceptions;
using ISL.Providers.ReIdentification.OfflineFileSources.Models.Providers.Exceptions;
using ISL.Providers.ReIdentification.OfflineFileSources.Services.Foundations.ReIdentifications;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Providers.OfflineFileSources
{
    internal class OfflineFileSourceProvider : IOfflineFileSourceProvider
    {
        private IReIdentificationService reIdentificationService { get; set; }

        public OfflineFileSourceProvider(OfflineSourceConfiguration offlineSourceConfiguration)
        {
            IServiceProvider serviceProvider = RegisterServices(offlineSourceConfiguration);
            InitializeClients(serviceProvider);
        }

        /// <summary>
        /// Re-identifies a patient from a list of pseudo identifiers.
        /// </summary>
        /// <returns>
        /// A re-identification request where the pseudo identfiers has been replaced by real identifiers.
        /// If the re-identification could not happen due to pseudo identifiers being valid, the identifier will be
        /// replaced by 0000000000 and the message field will be populated with a reason.
        /// </returns>
        /// <exception cref="OfflineFileSourceProviderValidationException" />
        /// <exception cref="OfflineFileSourceProviderDependencyValidationException" />
        /// <exception cref="OfflineFileSourceProviderDependencyException" />
        /// <exception cref="OfflineFileSourceProviderServiceException" />
        public async ValueTask<ReIdentificationRequest> ReIdentifyAsync(
            ReIdentificationRequest reIdentificationRequest)
        {
            try
            {
                return await reIdentificationService.ProcessReIdentificationRequest(reIdentificationRequest);
            }
            catch (ReIdentificationValidationException reIdentificationValidationException)
            {
                throw CreateProviderValidationException(
                    reIdentificationValidationException.InnerException as Xeption);
            }
            catch (ReIdentificationDependencyValidationException reIdentificationDependencyValidationException)
            {
                throw CreateProviderDependencyValidationException(
                    reIdentificationDependencyValidationException.InnerException as Xeption);
            }
            catch (ReIdentificationDependencyException reIdentificationDependencyException)
            {
                throw CreateProviderDependencyException(
                    reIdentificationDependencyException.InnerException as Xeption);
            }
            catch (ReIdentificationServiceException reIdentificationServiceException)
            {
                throw CreateProviderServiceException(
                    reIdentificationServiceException.InnerException as Xeption);
            }
        }

        private static OfflineFileSourceProviderValidationException CreateProviderValidationException(
            Xeption innerException)
        {
            return new OfflineFileSourceProviderValidationException(
                message: "Offline file source provider validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);
        }

        private static OfflineFileSourceProviderDependencyValidationException CreateProviderDependencyValidationException(
            Xeption innerException)
        {
            return new OfflineFileSourceProviderDependencyValidationException(
                message: "Offline file source provider dependency validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);
        }

        private static OfflineFileSourceProviderDependencyException CreateProviderDependencyException(
            Xeption innerException)
        {
            return new OfflineFileSourceProviderDependencyException(
                message: "Offline file source provider dependency error occurred, contact support.",
                innerException);
        }

        private static OfflineFileSourceProviderServiceException CreateProviderServiceException(Xeption innerException)
        {
            return new OfflineFileSourceProviderServiceException(
                message: "Offline file source provider service error occurred, contact support.",
                innerException);
        }

        private void InitializeClients(IServiceProvider serviceProvider) =>
            this.reIdentificationService = serviceProvider.GetRequiredService<IReIdentificationService>();

        private static IServiceProvider RegisterServices(OfflineSourceConfiguration offlineSourceConfiguration)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<IOfflineSourceBroker, OfflineSourceBroker>()
                .AddTransient<IReIdentificationService, ReIdentificationService>()
                .AddSingleton(offlineSourceConfiguration);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
