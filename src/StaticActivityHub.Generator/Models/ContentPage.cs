// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
namespace StaticActivityHub.Generator.Models;

internal sealed class ContentPage : BasicPage
{
   public override PageType PageType => PageType.Content;

   public required string RenderedContent { get; init; }
}
