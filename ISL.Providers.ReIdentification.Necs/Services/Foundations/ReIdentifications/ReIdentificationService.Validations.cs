// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Models.Foundations.ReIdentifications.Exceptions;

namespace ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationService
    {
        private static void ValidateIdentificationRequestOnProcess(ReIdentificationRequest reIdentificationRequest)
        {
            ValidateIdentificationRequestIsNotNull(reIdentificationRequest);

            Validate(
                (Rule: IsInvalid(reIdentificationRequest.ReIdentificationItems),
                Parameter: nameof(ReIdentificationRequest.ReIdentificationItems)),

                (Rule: IsInvalid(reIdentificationRequest.UserIdentifier),
                Parameter: nameof(ReIdentificationRequest.UserIdentifier)),

                (Rule: IsInvalid(reIdentificationRequest.Organisation),
                Parameter: nameof(ReIdentificationRequest.Organisation)),

                (Rule: IsInvalid(reIdentificationRequest.Reason),
                Parameter: nameof(ReIdentificationRequest.Reason)),

                (Rule: IsNotUnique(reIdentificationRequest.ReIdentificationItems),
                Parameter: nameof(ReIdentificationRequest.ReIdentificationItems)));
        }

        private static void ValidateIdentificationRequestIsNotNull(ReIdentificationRequest reIdentificationRequest)
        {
            if (reIdentificationRequest is null)
            {
                throw new NullReIdentificationRequestException("Identification request is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalid(List<ReIdentificationItem> reIdentificationItems) => new
        {
            Condition = reIdentificationItems is null || reIdentificationItems.Count == 0,
            Message = "Items is invalid"
        };

        private static dynamic IsNotUnique(List<ReIdentificationItem> reIdentificationItems) => new
        {
            Condition = IsNotUniqueList(reIdentificationItems),
            Message = "Items is invalid.  There are duplicate RowNumbers."
        };

        private static bool IsNotUniqueList(List<ReIdentificationItem> reIdentificationItems)
        {
            return reIdentificationItems is not null
                && reIdentificationItems.Count >= 0
                && reIdentificationItems.Select(item => item.RowNumber)
                    .Distinct().Count() != reIdentificationItems.Count();
        }

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;


        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidIdentificationRequestException =
                new InvalidReIdentificationRequestException(
                    message: "Invalid re-identification request.  Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidIdentificationRequestException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidIdentificationRequestException.ThrowIfContainsErrors();
        }
    }
}
