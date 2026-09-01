// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;

namespace StaticWebHub.Generator.Validation;

internal sealed class ValidationResultBuilder
{
   public void AddWarning(string code, string message, string? pageId = null)
   {
      _messages.Add(new ValidationMessage(
         ValidationSeverity.Warning, code, message, pageId));
   }

   public void AddError(string code, string message, string? pageId = null)
   {
      _messages.Add(new ValidationMessage(
         ValidationSeverity.Error, code, message, pageId));
   }

   public ValidationResult Build()
   {
      return new ValidationResult(_messages);
   }

   private readonly List<ValidationMessage> _messages = [];
}