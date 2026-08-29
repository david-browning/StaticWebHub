// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
namespace StaticActivityHub.Generator.Models;

internal sealed class RedirectPage : BasicPage
{
   public override PageType PageType => PageType.Redirect;

   public required string Destination { get; init; }

   public bool Permanent { get; init; } = true;
}
