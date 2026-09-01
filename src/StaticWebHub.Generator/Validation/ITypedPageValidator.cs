// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal interface ITypedPageValidator
{
   Type PageType { get; }

   Task<ValidationResult> ValidatePageAsync(
      BasicPage page,
      CancellationToken cancellationToken = default);
}
