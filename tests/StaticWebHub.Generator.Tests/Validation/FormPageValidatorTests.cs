// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using StaticWebHub.Generator.Tests.Infrastructure;
using StaticWebHub.Generator.Tests.TestData;
using StaticWebHub.Generator.Validation;

namespace StaticWebHub.Generator.Tests.Validation;

[TestClass]
public sealed class FormPageValidatorTests
{
   [TestMethod]
   public async Task DuplicateFieldIds_ReturnsDuplicateValueError()
   {
      var page = TestPages.CreateFormPage(
         fields:
         [
            TestPages.CreateTextField(
               id: "duplicate",
               label: "First"),
            TestPages.CreateTextField(
               id: "duplicate",
               label: "Second"),
         ]);

      var validator = new FormPageValidator();
      var result = await validator.ValidatePageAsync(page);
      ValidationAssert.HasError(
         result, ValidationCodes.DuplicateValue, page.Id);
   }

   // Next:
   // - Empty submit URL/text.
   // - Invalid/protocol-relative/unsupported submit URL.
   // - Empty field list warning.
   // - Missing/duplicate field IDs and missing label.
   // - Number min/max consistency.
   // - Select/radio options and duplicate option values.
   // - Values that are ignored by a field type.
}
