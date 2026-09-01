// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using StaticWebHub.Generator.Validation;

namespace StaticWebHub.Generator;

internal sealed class GenerationResult
{
   public required ValidationResult Validation { get; init; }

   public bool Succeeded => !Validation.HasErrors;
}