// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal class LauncherPageValidator : PageValidator<LauncherPage>
{
   protected override Task<ValidationResult> ValidateTypedPageAsync(
      LauncherPage page,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      var builder = new ValidationResultBuilder();
      if (page.Activities.Count == 0)
      {
         builder.AddWarning(
            ValidationCodes.RequiredCollectionEmpty,
            "The launcher contains no activities.",
            page.Id);
      }

      var activityIds = new HashSet<string>(StringComparer.Ordinal);

      foreach (var activity in page.Activities)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ValidateLauncherActivity(
            activity, page, activityIds, builder);
      }

      return Task.FromResult(builder.Build());
   }

   private void ValidateLauncherActivity(
      LauncherActivity activity,
      LauncherPage page,
      HashSet<string> activityIds,
      ValidationResultBuilder builder)
   {
      if (string.IsNullOrWhiteSpace(activity.Id))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            "Launcher activity is missing an Id.",
            page.Id);
      }
      else if (!activityIds.Add(activity.Id))
      {
         builder.AddError(
            ValidationCodes.DuplicateValue,
            $"Launcher activity Id \"{activity.Id}\" is used more than once.",
            page.Id);
      }

      if (string.IsNullOrWhiteSpace(activity.Title))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            "Launcher activity is missing a title.",
            page.Id);
      }

      if (string.IsNullOrWhiteSpace(activity.Description))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Launcher activity \"{activity.Title}\" is missing a description.",
            page.Id);
      }

      if (string.IsNullOrWhiteSpace(activity.Prompt))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Launcher activity \"{activity.Title}\" is missing a prompt.",
            page.Id);
      }
   }
}