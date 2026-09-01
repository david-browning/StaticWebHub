// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal class HubPageValidator : PageValidator<HubPage>
{
   protected override async Task<ValidationResult> ValidateTypedPageAsync(
      HubPage page,
      CancellationToken cancellationToken = default)
   {
      var builder = new ValidationResultBuilder();
      if (page.Items.Count == 0)
      {
         builder.AddWarning(
            ValidationCodes.RequiredCollectionEmpty,
            "The hub contains no items.",
            page.Id);
      }

      foreach (var item in page.Items)
      {
         ValidateHubItem(item, page, builder);
      }

      return builder.Build();
   }

   private void ValidateHubItem(
      HubItem item,
      HubPage page,
      ValidationResultBuilder builder)
   {
      if (string.IsNullOrWhiteSpace(item.Title))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Hub item is missing a title.",
            page.Id);
      }

      if (string.IsNullOrWhiteSpace(item.Description))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Hub item \"{item.Title}\" is missing a description.",
            page.Id);
      }

      if (string.IsNullOrWhiteSpace(item.Target))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Hub item \"{item.Title}\" is missing a target link.",
            page.Id);
      }
   }
}
