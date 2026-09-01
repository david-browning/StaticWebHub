// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace StaticWebHub.Generator.Models;

internal sealed class LauncherPage : BasicPage
{
   public override PageType PageType => PageType.Launcher;

   public IReadOnlyList<string> Instructions { get; init; } = [];

   public required IReadOnlyList<LauncherActivity> Activities { get; init; }
}

internal sealed class LauncherActivity
{
   public required string Id { get; init; }

   public required string Title { get; init; }

   public required string Description { get; init; }

   public IReadOnlyList<string> Tags { get; init; } = [];

   public string? Tip { get; init; }

   public required string Prompt { get; init; }

   public string? Provider { get; init; }
}
