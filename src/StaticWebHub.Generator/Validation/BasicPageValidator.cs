// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Content;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal class BasicPageValidator : PageValidator<BasicPage>
{
   public BasicPageValidator(
      IContentStore contentStore)
   {
      _contentStore = contentStore;
   }

   protected override async Task<ValidationResult> ValidateTypedPageAsync(
      BasicPage page,
      CancellationToken cancellationToken = default)
   {
      var builder = new ValidationResultBuilder();

      if (string.IsNullOrEmpty(page.Id))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"A page is missing an Id. Future errors may not be helpful.");
      }

      if (string.IsNullOrEmpty(page.Locale))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Page is missing its locale.",
            page.Id);
      }

      if (string.IsNullOrEmpty(page.Title))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Page is missing its title.",
            page.Id);
      }

      if (string.IsNullOrEmpty(page.Subtitle))
      {
         builder.AddWarning(
            ValidationCodes.DiscouragedValue,
            $"Page does not have a subtitle.",
            page.Id);
      }

      if (string.IsNullOrEmpty(page.Description))
      {
         builder.AddWarning(
            ValidationCodes.DiscouragedValue,
            $"Page does not have a description.",
            page.Id);
      }

      if (string.IsNullOrEmpty(page.Slug))
      {
         builder.AddWarning(
            ValidationCodes.DiscouragedValue,
            $"Page does not have a slug. The Id will be used instead.",
            page.Id);
      }

      if (page.Order < 0)
      {
         builder.AddError(
            ValidationCodes.ValueOutOfRange,
            $"Order is negative.",
            page.Id);
      }

      if (!page.IsPublished)
      {
         builder.AddWarning(
            ValidationCodes.NotPublished,
            "\"IsPublished\" set to false.",
            page.Id);
      }

      var ret = builder.Build();
      ret.Combine(await ValidateScriptsAsync(page, cancellationToken));
      ret.Combine(await ValidateStylesheetsAsync(page, cancellationToken));
      return ret;
   }

   private async Task<ValidationResult> ValidateScriptsAsync(
      BasicPage page,
      CancellationToken cancellationToken)
   {
      var builder = new ValidationResultBuilder();
      var assetKeys = new HashSet<string>(StringComparer.Ordinal);

      foreach (var script in page.Scripts)
      {
         cancellationToken.ThrowIfCancellationRequested();

         if (string.IsNullOrWhiteSpace(script.AssetKey))
         {
            builder.AddError(
               ValidationCodes.RequiredValueEmpty,
               "A script reference is missing its asset key.",
               page.Id);

            continue;
         }

         if (!IsLocalAssetKey(script.AssetKey))
         {
            builder.AddError(
               ValidationCodes.InvalidAssetKey,
               $"Script asset key \"{script.AssetKey}\" is not a valid local asset key.",
               page.Id);

            continue;
         }

         if (!assetKeys.Add(script.AssetKey))
         {
            builder.AddWarning(
               ValidationCodes.DuplicateAssetReference,
               $"Script asset \"{script.AssetKey}\" is referenced more than once.",
               page.Id);
         }

         await ValidateAssetAsync(
            script.AssetKey,
            "text/javascript",
            "script",
            page,
            builder,
            cancellationToken);
      }

      return builder.Build();
   }

   private async Task<ValidationResult> ValidateStylesheetsAsync(
      BasicPage page,
      CancellationToken cancellationToken)
   {
      var builder = new ValidationResultBuilder();
      var assetKeys = new HashSet<string>(StringComparer.Ordinal);

      foreach (var stylesheet in page.StyleSheets)
      {
         cancellationToken.ThrowIfCancellationRequested();

         if (string.IsNullOrWhiteSpace(stylesheet.AssetKey))
         {
            builder.AddError(
               ValidationCodes.RequiredValueEmpty,
               "A stylesheet reference is missing its asset key.",
               page.Id);

            continue;
         }

         if (!IsLocalAssetKey(stylesheet.AssetKey))
         {
            builder.AddError(
               ValidationCodes.InvalidAssetKey,
               $"Stylesheet asset key \"{stylesheet.AssetKey}\" is not a valid local asset key.",
               page.Id);

            continue;
         }

         if (!assetKeys.Add(stylesheet.AssetKey))
         {
            builder.AddWarning(
               ValidationCodes.DuplicateAssetReference,
               $"Stylesheet asset \"{stylesheet.AssetKey}\" is referenced more than once.",
               page.Id);
         }

         await ValidateAssetAsync(
            stylesheet.AssetKey,
            "text/css",
            "stylesheet",
            page,
            builder,
            cancellationToken);
      }

      return builder.Build();
   }

   private async Task ValidateAssetAsync(
      string assetKey,
      string expectedContentType,
      string assetDescription,
      BasicPage page,
      ValidationResultBuilder builder,
      CancellationToken cancellationToken)
   {
      try
      {
         var asset = await _contentStore.GetAssetAsync(
            assetKey, cancellationToken);

         var contentType = AssetHelpers.GetMediaType(asset.ContentType);

         if (!string.Equals(
            contentType, expectedContentType, StringComparison.OrdinalIgnoreCase))
         {
            builder.AddError(
               ValidationCodes.InvalidAssetContentType,
               $"The {assetDescription} asset \"{assetKey}\" has content type " +
               $"\"{contentType}\" instead of \"{expectedContentType}\".",
               page.Id);
         }
      }
      catch (FileNotFoundException)
      {
         builder.AddError(
            ValidationCodes.AssetNotFound,
            $"The {assetDescription} asset \"{assetKey}\" does not exist.",
            page.Id);
      }
      catch (ArgumentException)
      {
         builder.AddError(
            ValidationCodes.InvalidAssetKey,
            $"The {assetDescription} asset key \"{assetKey}\" is invalid.",
            page.Id);
      }
      catch (UnauthorizedAccessException)
      {
         builder.AddError(
            ValidationCodes.AssetReadFailed,
            $"The {assetDescription} asset \"{assetKey}\" could not be read.",
            page.Id);
      }
      catch (IOException)
      {
         builder.AddError(
            ValidationCodes.AssetReadFailed,
            $"The {assetDescription} asset \"{assetKey}\" could not be read.",
            page.Id);
      }
   }

   private static bool IsLocalAssetKey(string assetKey)
   {
      if (assetKey.StartsWith('/') ||
         assetKey.StartsWith('\\') ||
         Path.IsPathRooted(assetKey))
      {
         return false;
      }

      if (Uri.TryCreate(assetKey, UriKind.Absolute, out _))
      {
         return false;
      }

      var segments = assetKey.Split(new[] { '/', '\\' },
         StringSplitOptions.RemoveEmptyEntries);

      foreach (var segment in segments)
      {
         if (string.Equals(segment, "..", StringComparison.Ordinal))
         {
            return false;
         }
      }

      return true;
   }

   private readonly IContentStore _contentStore;
}
