// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal abstract class PageValidator<TPage> : ITypedPageValidator
   where TPage : BasicPage
{
   public Type PageType => typeof(TPage);

   public Task<ValidationResult> ValidatePageAsync(
      BasicPage page,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(page);

      if (page is not TPage typedPage)
      {
         throw new ArgumentException(
            $"Validator for {typeof(TPage).Name} " +
            $"cannot validate {page.GetType().Name}.",
            nameof(page));
      }

      return ValidateTypedPageAsync(typedPage, cancellationToken);
   }

   /// <summary>
   /// Classes that inherit from this must implement this function to perform
   /// the actual validation.
   /// </summary>
   /// <param name="page"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   protected abstract Task<ValidationResult> ValidateTypedPageAsync(
      TPage page,
      CancellationToken cancellationToken = default);
}