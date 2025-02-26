// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.DemoData.Models;

namespace ISL.Providers.ReIdentification.DemoData.Services.Foundations.ReIdentifications
{
    internal partial class ReIdentificationService : IReIdentificationService
    {
        private readonly DemoDataReIdentificationConfigurations demoDataReIdentificationConfigurations;

        public ReIdentificationService(
            DemoDataReIdentificationConfigurations demoDataReIdentificationConfigurations)
        {
            this.demoDataReIdentificationConfigurations = demoDataReIdentificationConfigurations;
        }

        public ValueTask<ReIdentificationRequest> ProcessReIdentificationRequest(
            ReIdentificationRequest reIdentificationRequest) =>
            TryCatch(async () =>
            {
                ValidateIdentificationRequestOnProcess(reIdentificationRequest);

                var returnItems = reIdentificationRequest.DeepClone();

                foreach (var item in returnItems.ReIdentificationItems)
                {
                    if (string.IsNullOrWhiteSpace(item.RowNumber))
                    {
                        item.Identifier = demoDataReIdentificationConfigurations.DefaultIdentifier;
                        item.Message = "Each identifier must have a corresponding row number.";
                        continue;
                    }

                    if (
                        string.IsNullOrWhiteSpace(item.Identifier)
                        || item.Identifier.Length > 15
                        || item.Identifier.All(char.IsDigit) is false)
                    {
                        item.Identifier = demoDataReIdentificationConfigurations.DefaultIdentifier;
                        item.Message = "Identifier must be 15 digits or less.";
                        continue;
                    }

                    var demoNhsNumber = demoDataReIdentificationConfigurations.DemoPrefix +
                        item.Identifier.PadLeft(7, '0');

                    item.Identifier = demoNhsNumber;
                    item.Message = "OK";
                }

                return returnItems;
            });
    }
}
