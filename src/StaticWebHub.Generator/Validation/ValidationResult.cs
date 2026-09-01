// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;

namespace StaticWebHub.Generator.Validation;

internal enum ValidationSeverity
{
   Warning,
   Error
}

internal sealed record ValidationMessage(
   ValidationSeverity Severity,
   string Code,
   string Message,
   string? PageId = null);

internal class ValidationResult
{
   public ValidationResult()
   {
      Messages = new List<ValidationMessage>();
   }

   public ValidationResult(IEnumerable<ValidationMessage> messages)
   {
      ArgumentNullException.ThrowIfNull(messages);
      Messages = messages.ToList();
   }

   public void Combine(ValidationResult result)
   {
      Messages.AddRange(result.Messages);
   }

   public List<ValidationMessage> Messages { get; }

   public IEnumerable<ValidationMessage> Errors => 
      Messages.Where(message => message.Severity == ValidationSeverity.Error);

   public IEnumerable<ValidationMessage> Warnings => 
      Messages.Where(message => message.Severity == ValidationSeverity.Warning);

   public bool HasErrors => 
      Messages.Any(message => message.Severity == ValidationSeverity.Error);

   public static ValidationResult Success { get; } = new([]);
}
