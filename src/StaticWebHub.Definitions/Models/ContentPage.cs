// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
namespace StaticWebHub.Definitions.Models;

public sealed class ContentPage : BasicPage
{
   public override PageType PageType => PageType.Content;

   public required string RenderedContent { get; init; }
}
