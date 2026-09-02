// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Linq;
using StaticWebHub.Generator.Validation;

namespace StaticWebHub.Generator.Tests.Infrastructure;

internal static class ValidationAssert
{
   public static void HasNoErrors(
      ValidationResult result)
   {
      var errors = result.Errors.ToArray();

      Assert.AreEqual(
         0,
         errors.Length,
         $"Expected no validation errors. Actual codes: " +
         $"{string.Join(", ", errors.Select(item => item.Code))}");
   }

   public static ValidationMessage HasError(
      ValidationResult result,
      string code,
      string? pageId = null)
   {
      var message = result.Errors.FirstOrDefault(
         item =>
            item.Code == code &&
            (pageId is null || item.PageId == pageId));

      Assert.IsNotNull(
         message,
         $"Expected error '{code}'" +
         (pageId is null ? "." : $" for page '{pageId}'."));

      return message!;
   }

   public static ValidationMessage HasWarning(
      ValidationResult result,
      string code,
      string? pageId = null)
   {
      var message = result.Warnings.FirstOrDefault(
         item =>
            item.Code == code &&
            (pageId is null || item.PageId == pageId));

      Assert.IsNotNull(
         message,
         $"Expected warning '{code}'" +
         (pageId is null ? "." : $" for page '{pageId}'."));

      return message!;
   }

   public static void DoesNotContainCode(
      ValidationResult result,
      string code)
   {
      Assert.IsFalse(
         result.Messages.Any(message => message.Code == code),
         $"Did not expect validation code '{code}'.");
   }
}
