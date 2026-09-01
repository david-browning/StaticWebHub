// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal interface IPageValidator
{
   Task<ValidationResult> ValidateAsync(
      BasicPage page,
      CancellationToken cancellationToken = default);
}
