// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StaticWebHub.Generator.Models;

[JsonPolymorphic(
   TypeDiscriminatorPropertyName = "viewType")]
[JsonDerivedType(typeof(HubPage), "hub")]
[JsonDerivedType(typeof(LauncherPage), "launcher")]
[JsonDerivedType(typeof(FormPage), "form")]
[JsonDerivedType(typeof(ContentPage), "content")]
[JsonDerivedType(typeof(RedirectPage), "redirect")]
internal abstract class BasicPage
{
   public required string Id { get; init; }

   public required string Locale { get; init; }

   public required string Title { get; init; }

   public string? Subtitle { get; init; }

   public string? Description { get; init; }

   [JsonIgnore]
   public abstract PageType PageType { get; }

   public string? Slug { get; init; }

   public int Order { get; init; }

   public bool IsPublished { get; init; } = true;

   public IReadOnlyList<ScriptLinkReference> Scripts { get; init; } = [];

   public IReadOnlyList<StylesheetLinkReference> StyleSheets { get; init; } = [];
}

internal enum PageType
{
   Hub,
   Launcher,
   Form,
   Content,
   Redirect
}