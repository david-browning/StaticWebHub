// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal class RedirectPageValidator : PageValidator<RedirectPage>
{
   protected override async Task<ValidationResult> ValidateTypedPageAsync(
      RedirectPage page,
      CancellationToken cancellationToken = default)
   {
      var builder = new ValidationResultBuilder();
      if (string.IsNullOrWhiteSpace(page.Destination))
      {
         builder.AddError(
            ValidationCodes.RequiredFieldMissing,
            "The page destination property is missing.",
            page.Id);

         // No more validation
         return builder.Build();
      }

      // If its a local path:
      if (page.Destination.StartsWith('/'))
      {
         ValidateLocalPath(page, builder);
         return builder.Build();
      }

      // At this point, the destination is a URL. Try to parse it.
      if (!Uri.TryCreate(page.Destination, UriKind.Absolute, out var uri))
      {
         builder.AddError(
            ValidationCodes.InvalidUrl,
            $"Page has invalid destination",
            page.Id);

         // No more validation
         return builder.Build();
      }

      // At this point, the URI is an internet path:
      if(uri.Scheme == Uri.UriSchemeHttp)
      {
         builder.AddWarning(
            ValidationCodes.DiscouragedValue,
            "Redirect page uses an unsecure HTTP path. Recommend HTTPS only.",
            page.Id);
      }

      if (uri.Scheme != Uri.UriSchemeHttps)
      {
         builder.AddError(
            ValidationCodes.InvalidUrl,
            "Page uses an unsupported URI scheme. " +
            "Only HTTPS external redirects are supported.");
      }

      return builder.Build();
   }

   private void ValidateLocalPath(
      RedirectPage page,
      ValidationResultBuilder result)
   {
      if (page.Destination.StartsWith("//"))
      {
         result.AddError(
            ValidationCodes.InvalidUrl,
            "Page uses a protocol-relative URL. " +
            "Use an explicit HTTPS URL instead.",
            page.Id);
      }

      if (page.Destination.Contains('\\'))
      {
         result.AddError(
            ValidationCodes.InvalidUrl,
            "Page contains an invalid backslash in local destination. " +
            "Replace with forward slashes.",
            page.Id);
      }
      
      // Cannot check if the redirect page exists because the page may not
      // have been generated yet.

      // TODO: Check for redirect cycles or a self redirect?
   }
}
