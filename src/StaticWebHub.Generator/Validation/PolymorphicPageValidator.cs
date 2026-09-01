// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal sealed class PolymorphicPageValidator : IPageValidator
{
   public PolymorphicPageValidator(IEnumerable<ITypedPageValidator> validators)
   {
      ArgumentNullException.ThrowIfNull(validators);
      _validators = validators.ToArray();
   }

   public async Task<ValidationResult> ValidateAsync(
      BasicPage page,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(page);
      var ret = new ValidationResult();
      var pageType = page.GetType();

      var validators = _validators
         .Where(validator => validator.PageType.IsAssignableFrom(pageType))
         .OrderBy(validator => GetInheritanceDepth(validator.PageType));

      foreach (var validator in validators)
      {
         cancellationToken.ThrowIfCancellationRequested();
         var result = await validator.ValidatePageAsync(page, cancellationToken);
         ret.Combine(result);
      }

      return ret;
   }

   private static int GetInheritanceDepth(Type type)
   {
      var depth = 0;

      for (var cur = type; cur.BaseType is not null; cur = cur.BaseType)
      {
         depth++;
      }

      return depth;
   }

   private readonly IReadOnlyList<ITypedPageValidator> _validators;
}
