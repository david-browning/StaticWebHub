// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
namespace StaticWebHub.Generator.Models;

internal class ScriptLinkReference
{
   public required string AssetKey { get; init; }

   public bool Defer { get; init; } = true;
}
