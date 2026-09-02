// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT

namespace StaticWebHub.Generator.Tests.Generation;

[TestClass]
public sealed class SiteGeneratorTests
{
   // Next:
   // - Valid pages run the whole in-process pipeline.
   // - Unpublished pages are skipped.
   // - Diagnostics accumulate across pages.
   // - Error behavior does not leave an unintended partial site.
   // - Deserialization errors include source path.
}
