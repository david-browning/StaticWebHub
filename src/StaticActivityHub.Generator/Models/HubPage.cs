// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace StaticActivityHub.Generator.Models;

internal sealed class HubPage : BasicPage
{
   public override PageType PageType => PageType.Hub;

   public required IReadOnlyList<HubItem> Items { get; init; }
}

internal sealed class HubItem
{
   public required string Title { get; init; }

   public required string Description { get; init; }

   public required string Target { get; init; }

   public string? Icon { get; init; }

   public IReadOnlyList<string> Tags { get; init; } = [];
}
