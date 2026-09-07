// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT

using System.Collections.Generic;

namespace StaticWebHub.Definitions.Models;

public sealed class SiteConfiguration
{
   public required string Title { get; init; }

   public required string DefaultLocale { get; init; }

   public required IReadOnlyList<SiteLocale> Locales { get; init; }
}

public sealed class SiteLocale
{
   public required string Code { get; init; }

   public required string DisplayName { get; init; }

   public required string HomePage { get; init; }
}