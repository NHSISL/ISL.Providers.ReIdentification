// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.DemoData.Models.Foundations.ReIdentifications.Exceptions;

namespace ISL.Providers.ReIdentification.DemoData.Services.Foundations.ReIdentifications
{
    internal partial class ReIdentificationService
    {
        virtual internal void ValidateIdentificationRequestOnProcess(ReIdentificationRequest reIdentificationRequest)
        {
            ValidateIdentificationRequestIsNotNull(reIdentificationRequest);

            Validate(
                (Rule: IsInvalid(reIdentificationRequest.RequestId),
                Parameter: nameof(ReIdentificationRequest.RequestId)),

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

        private static void ValidateIdentificationRequestItem(ReIdentificationItem reIdentificationItem)
        {
            Validate(
                (Rule: IsInvalidRowNumber(reIdentificationItem.RowNumber),
                Parameter: nameof(ReIdentificationItem.RowNumber)),

                (Rule: IsInvalidIdentifier(reIdentificationItem.Identifier),
                Parameter: nameof(ReIdentificationItem.Identifier)));
        }


        private static void ValidateIdentificationRequestIsNotNull(ReIdentificationRequest reIdentificationRequest)
        {
            if (reIdentificationRequest is null)
            {
                throw new NullReIdentificationRequestException("Re-identification request is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalidRowNumber(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Each identifier must have a corresponding row number."
        };

        private static dynamic IsInvalidIdentifier(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name) || IsExactTenDigits(name) is false,
            Message = "Text must be exactly 10 digits."
        };

        private static bool IsExactTenDigits(string input)
        {
            bool result = input.Length == 10 && input.All(char.IsDigit);

            return result;
        }

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
