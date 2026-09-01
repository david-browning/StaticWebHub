// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT

using System.Collections.Generic;

namespace StaticWebHub.Generator.Models;

internal sealed class SiteIndexViewModel
{
   public required string Title { get; init; }

   public required IReadOnlyList<SiteIndexLocale> Locales { get; init; }
}

internal sealed class SiteIndexLocale
{
   public required string DisplayName { get; init; }

   public required string Url { get; init; }

   public bool IsDefault { get; init; }
}