﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.DemoData.Models;
using ISL.Providers.ReIdentification.DemoData.Models.Foundations.ReIdentifications.Exceptions;
using ISL.Providers.ReIdentification.DemoData.Models.Providers.Exceptions;
using ISL.Providers.ReIdentification.DemoData.Services.Foundations.ReIdentifications;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace ISL.Providers.ReIdentification.DemoData.Providers.DemoData
{
    public class DemoDataReIdentificationProvider : IDemoDataReIdentificationProvider
    {
        private IReIdentificationService reIdentificationService { get; set; }

        public DemoDataReIdentificationProvider(
            DemoDataReIdentificationConfigurations offlineSourceReIdentificationConfiguration)
        {
            IServiceProvider serviceProvider = RegisterServices(offlineSourceReIdentificationConfiguration);
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
        /// <exception cref="DemoDataProviderValidationException" />
        /// <exception cref="DemoDataProviderDependencyValidationException" />
        /// <exception cref="DemoDataProviderDependencyException" />
        /// <exception cref="DemoDataProviderServiceException" />
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

        private static DemoDataProviderValidationException CreateProviderValidationException(
            Xeption innerException)
        {
            return new DemoDataProviderValidationException(
                message: "Offline file source provider validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);
        }

        private static DemoDataProviderDependencyValidationException CreateProviderDependencyValidationException(
            Xeption innerException)
        {
            return new DemoDataProviderDependencyValidationException(
                message: "Offline file source provider dependency validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);
        }

        private static DemoDataProviderDependencyException CreateProviderDependencyException(
            Xeption innerException)
        {
            return new DemoDataProviderDependencyException(
                message: "Offline file source provider dependency error occurred, contact support.",
                innerException);
        }

        private static DemoDataProviderServiceException CreateProviderServiceException(Xeption innerException)
        {
            return new DemoDataProviderServiceException(
                message: "Offline file source provider service error occurred, contact support.",
                innerException);
        }

        private void InitializeClients(IServiceProvider serviceProvider) =>
            reIdentificationService = serviceProvider.GetRequiredService<IReIdentificationService>();

        private static IServiceProvider RegisterServices(
            DemoDataReIdentificationConfigurations offlineSourceReIdentificationConfiguration)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<IReIdentificationService, ReIdentificationService>()
                .AddSingleton(offlineSourceReIdentificationConfiguration);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
