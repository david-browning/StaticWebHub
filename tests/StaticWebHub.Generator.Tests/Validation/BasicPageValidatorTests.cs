// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using StaticWebHub.Generator.Tests.Infrastructure;
using StaticWebHub.Generator.Tests.TestData;
using StaticWebHub.Generator.Validation;

namespace StaticWebHub.Generator.Tests.Validation;

[TestClass]
public sealed class BasicPageValidatorTests
{
   [TestMethod]
   public async Task EmptyId_ReturnsRequiredValueEmptyError()
   {
      var contentStore = new InMemoryContentStore();
      var validator = new BasicPageValidator(contentStore);
      var page = TestPages.CreateContentPage(id: string.Empty);
      var result = await validator.ValidatePageAsync(page);
      ValidationAssert.HasError(result, ValidationCodes.RequiredValueEmpty);
   }

   // Next:
   // - Valid common metadata has no errors.
   // - Missing locale/title.
   // - Negative order.
   // - IsPublished=false warning.
   // - Script/stylesheet missing, bad key, wrong MIME, duplicate reference.
}
