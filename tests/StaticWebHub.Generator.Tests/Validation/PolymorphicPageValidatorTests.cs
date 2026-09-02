// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT

namespace StaticWebHub.Generator.Tests.Validation;

[TestClass]
public sealed class PolymorphicPageValidatorTests
{
   // Next:
   // - FormPage invokes BasicPageValidator + FormPageValidator.
   // - Unrelated validators do not run.
   // - Base-before-derived ordering.
   // - Results combine.
   // - Cancellation propagates.
   // - Reflection discovery finds every concrete ITypedPageValidator.
}
