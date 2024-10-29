// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Brokers.Identifiers;
using ISL.Providers.ReIdentification.Necs.Brokers.Necs;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Models.Foundations.ReIdentifications.Exceptions;
using ISL.Providers.ReIdentification.Necs.Models.Providers.Exceptions;
using ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace ISL.Providers.ReIdentification.Necs.Providers.NecsReIdentifications
{
    public class NecsReIdentificationProvider : INecsReIdentificationProvider
    {
        private IReIdentificationService reIdentificationService { get; set; }

        public NecsReIdentificationProvider(NecsReIdentificationConfigurations configurations)
        {
            IServiceProvider serviceProvider = RegisterServices(configurations);
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
        /// <exception cref="NecsReIdentificationProviderValidationException" />
        /// <exception cref="NecsReIdentificationProviderDependencyValidationException" />
        /// <exception cref="NecsReIdentificationProviderDependencyException" />
        /// <exception cref="NecsReIdentificationProviderServiceException" />
        public async ValueTask<ReIdentificationRequest> ReIdentifyAsync(ReIdentificationRequest reIdentificationRequest)
        {
            try
            {
                return await reIdentificationService.ProcessReIdentificationRequest(reIdentificationRequest);
            }
            catch (ReIdentificationValidationException notificationValidationException)
            {
                throw CreateProviderValidationException(
                    notificationValidationException.InnerException as Xeption);
            }
            catch (ReIdentificationDependencyValidationException notificationDependencyValidationException)
            {
                throw CreateProviderDependencyValidationException(
                    notificationDependencyValidationException.InnerException as Xeption);
            }
            catch (ReIdentificationDependencyException notificationDependencyException)
            {
                throw CreateProviderDependencyException(
                    notificationDependencyException.InnerException as Xeption);
            }
            catch (ReIdentificationServiceException notificationServiceException)
            {
                throw CreateProviderServiceException(
                    notificationServiceException.InnerException as Xeption);
            }
        }

        private static NecsReIdentificationProviderValidationException CreateProviderValidationException(
            Xeption innerException)
        {
            return new NecsReIdentificationProviderValidationException(
                message: "NECS re-identification provider validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);
        }

        private static NecsReIdentificationProviderDependencyValidationException
            CreateProviderDependencyValidationException(Xeption innerException)
        {
            return new NecsReIdentificationProviderDependencyValidationException(
                message: "NECS re-identification provider dependency validation error occurred, " +
                    "fix errors and try again.",
                innerException,
                data: innerException.Data);
        }

        private static NecsReIdentificationProviderDependencyException CreateProviderDependencyException(
            Xeption innerException)
        {
            return new NecsReIdentificationProviderDependencyException(
                message: "NECS re-identification provider dependency error occurred, contact support.",
                innerException);
        }

        private static NecsReIdentificationProviderServiceException CreateProviderServiceException(
            Xeption innerException)
        {
            return new NecsReIdentificationProviderServiceException(
                message: "NECS re-identification provider service error occurred, contact support.",
                innerException);
        }

        private void InitializeClients(IServiceProvider serviceProvider) =>
            reIdentificationService = serviceProvider.GetRequiredService<IReIdentificationService>();

        private static IServiceProvider RegisterServices(NecsReIdentificationConfigurations configurations)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<INECSBroker, NECSBroker>()
                .AddTransient<IIdentifierBroker, IdentifierBroker>()
                .AddTransient<IReIdentificationService, ReIdentificationService>()
                .AddSingleton(configurations);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }


    }
}
