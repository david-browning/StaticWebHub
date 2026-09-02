// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT

namespace StaticWebHub.Generator.Tests.Content;

[TestClass]
public sealed class LocalContentStoreTests
{
   // Use TemporaryDirectory because filesystem behavior is the subject.
   //
   // Next:
   // - Text/binary reads and metadata.
   // - Exists/open/write/delete.
   // - CreatedNew vs Overwritten.
   // - Rooted keys, parent traversal, and root escape prevention.
}
