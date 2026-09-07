// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
namespace StaticWebHub.Definitions.Models;

public class ScriptLinkReference
{
   public required string AssetKey { get; init; }

   public bool Defer { get; init; } = true;
}
