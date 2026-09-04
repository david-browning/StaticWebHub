// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal class FormPageValidator : PageValidator<FormPage>
{
   protected override Task<ValidationResult> ValidateTypedPageAsync(
      FormPage page,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      var builder = new ValidationResultBuilder();
      ValidateSubmitUrl(page, builder);

      if (string.IsNullOrWhiteSpace(page.SubmitText))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            "The form is missing submit button text.",
            page.Id);
      }

      // Property is supplied by default but check it in case the user 
      // overwrote it.
      if (string.IsNullOrWhiteSpace(page.FormId))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            "The form is missing an Id.",
            page.Id);
      }

      if (page.Fields.Count == 0)
      {
         builder.AddWarning(
            ValidationCodes.RequiredCollectionEmpty,
            "The form contains no fields.",
            page.Id);
      }

      var fieldIds = new HashSet<string>(StringComparer.Ordinal);
      foreach (var field in page.Fields)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ValidateFormField(field, page, fieldIds, builder);
      }

      return Task.FromResult(builder.Build());
   }

   private void ValidateSubmitUrl(
      FormPage page,
      ValidationResultBuilder builder)
   {
      if (string.IsNullOrWhiteSpace(page.SubmitUrl))
      {
         // User likely wants to use JavaScript instead of POSTing. Skip
         // additional submit validation.
         return;
      }

      if (page.SubmitUrl.StartsWith("//", StringComparison.Ordinal))
      {
         builder.AddError(
            ValidationCodes.InvalidUrl,
            $"Submit URL \"{page.SubmitUrl}\" is protocol-relative. " +
            "Use a local path or an explicit HTTPS URL.",
            page.Id);

         return;
      }

      if (!Uri.TryCreate(page.SubmitUrl, UriKind.RelativeOrAbsolute, out var uri))
      {
         builder.AddError(
            ValidationCodes.InvalidUrl,
            $"Submit URL \"{page.SubmitUrl}\" is not a valid URI.",
            page.Id);

         return;
      }

      if (uri.IsAbsoluteUri && uri.Scheme != Uri.UriSchemeHttps)
      {
         builder.AddError(
            ValidationCodes.InvalidUrl,
            $"Submit URL \"{page.SubmitUrl}\" uses unsupported scheme " +
            $"\"{uri.Scheme}\". Only HTTPS is supported.",
            page.Id);
      }
   }

   private void ValidateFormField(
      FormField field,
      FormPage page,
      HashSet<string> fieldIds,
      ValidationResultBuilder builder)
   {
      if (string.IsNullOrWhiteSpace(field.Id))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            "Form field is missing an Id.",
            page.Id);
      }
      else if (!fieldIds.Add(field.Id))
      {
         builder.AddError(
            ValidationCodes.DuplicateValue,
            $"Form field Id \"{field.Id}\" is used more than once.",
            page.Id);
      }

      if (string.IsNullOrWhiteSpace(field.Label))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Form field \"{field.Id}\" is missing a label.",
            page.Id);
      }

      if (!Enum.IsDefined(typeof(FormFieldType), field.Type))
      {
         builder.AddError(
            ValidationCodes.InvalidValue,
            $"Form field \"{field.Id}\" has invalid field type " +
            $"\"{field.Type}\".",
            page.Id);

         return;
      }

      ValidateFieldRange(field, page, builder);
      ValidateFieldOptions(field, page, builder);
      ValidateUnusedFieldValues(field, page, builder);
   }

   private void ValidateFieldRange(
      FormField field,
      FormPage page,
      ValidationResultBuilder builder)
   {
      if (field.Type == FormFieldType.Number)
      {
         if (field.Minimum.HasValue &&
            field.Maximum.HasValue &&
            field.Minimum.Value > field.Maximum.Value)
         {
            builder.AddError(
               ValidationCodes.ConflictingValues,
               $"Number field \"{field.Id}\" has minimum " +
               $"{field.Minimum.Value} greater than maximum " +
               $"{field.Maximum.Value}.",
               page.Id);
         }

         return;
      }

      if (field.Minimum.HasValue ||
         field.Maximum.HasValue)
      {
         builder.AddWarning(
            ValidationCodes.UnusedValue,
            $"Form field \"{field.Id}\" specifies a minimum or maximum, " +
            $"but its type is \"{field.Type}\".",
            page.Id);
      }
   }

   private void ValidateFieldOptions(
      FormField field,
      FormPage page,
      ValidationResultBuilder builder)
   {
      var supportsOptions =
         field.Type == FormFieldType.Select ||
         field.Type == FormFieldType.Radio;

      if (!supportsOptions)
      {
         if (field.Options.Count > 0)
         {
            builder.AddWarning(
               ValidationCodes.UnusedValue,
               $"Form field \"{field.Id}\" contains options, " +
               $"but its type is \"{field.Type}\".",
               page.Id);
         }

         return;
      }

      if (field.Options.Count == 0)
      {
         builder.AddError(
            ValidationCodes.RequiredCollectionEmpty,
            $"Form field \"{field.Id}\" requires at least one option.",
            page.Id);

         return;
      }

      var optionValues = new HashSet<string>(StringComparer.Ordinal);
      foreach (var option in field.Options)
      {
         ValidateFormOption(
            option,
            field,
            page,
            optionValues,
            builder);
      }
   }

   private void ValidateFormOption(
      FormOption option,
      FormField field,
      FormPage page,
      HashSet<string> optionValues,
      ValidationResultBuilder builder)
   {
      if (string.IsNullOrWhiteSpace(option.Value))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Form field \"{field.Id}\" contains an option with no value.",
            page.Id);
      }
      else if (!optionValues.Add(option.Value))
      {
         builder.AddError(
            ValidationCodes.DuplicateValue,
            $"Form field \"{field.Id}\" uses option value " +
            $"\"{option.Value}\" more than once.",
            page.Id);
      }

      if (string.IsNullOrWhiteSpace(option.Label))
      {
         builder.AddError(
            ValidationCodes.RequiredValueEmpty,
            $"Form field \"{field.Id}\" contains an option with no label.",
            page.Id);
      }
   }

   private void ValidateUnusedFieldValues(
      FormField field,
      FormPage page,
      ValidationResultBuilder builder)
   {
      if (string.IsNullOrWhiteSpace(field.Placeholder))
      {
         return;
      }

      if (field.Type == FormFieldType.Select ||
         field.Type == FormFieldType.Checkbox ||
         field.Type == FormFieldType.Radio)
      {
         builder.AddWarning(
            ValidationCodes.UnusedValue,
            $"Form field \"{field.Id}\" specifies a placeholder, " +
            $"but placeholders are not used for \"{field.Type}\" fields.",
            page.Id);
      }
   }
}