// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace StaticWebHub.Generator.Validation;

internal static class ValidationServiceExtensions
{
   public static IServiceCollection AddPageValidators(
      this IServiceCollection services)
   {
      ArgumentNullException.ThrowIfNull(services);
      var validatorInterface = typeof(ITypedPageValidator);
      var validatorTypes = validatorInterface.Assembly.GetTypes()
         .Where(type => type.Namespace is not null &&
         type.Namespace.StartsWith("StaticWebHub.Generator.Validation", StringComparison.Ordinal) &&
         !type.IsAbstract &&
         !type.IsInterface &&
         validatorInterface.IsAssignableFrom(type));
      foreach (var validatorType in validatorTypes)
      {
         services.AddTransient(validatorInterface, validatorType);
      }

      return services;
   }
}