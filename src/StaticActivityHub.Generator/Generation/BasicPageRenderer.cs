// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;
using Razor.Templating.Core;
using StaticActivityHub.Generator.Models;

namespace StaticActivityHub.Generator.Generation;

internal class BasicPageRenderer : IPageRenderer
{
   public Task<string> GenerateAsync(
      BasicPage page,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(page);
      cancellationToken.ThrowIfCancellationRequested();

      return page switch
      {
         HubPage hub => RenderHubPageAsync(hub),
         RedirectPage redirect => RenderRedirectPageAynsc(redirect),
         LauncherPage launcher => RenderLauncherPageAsync(launcher),
         FormPage form => RenderFormPageAsync(form),
         ContentPage content => RenderContentPageAsync(content),
         _ => throw new NotSupportedException(
            $"${page.Id} is not a supported type of page.")
      };
   }

   private static Task<string> RenderHubPageAsync(HubPage page)
   {
      return RazorTemplateEngine.RenderAsync(
         "/Views/HubPage.cshtml", page);
   }

   private static Task<string> RenderRedirectPageAynsc(RedirectPage page)
   {
      return RazorTemplateEngine.RenderAsync(
         "/Views/RedirectPage.cshtml", page);
   }

   private static Task<string> RenderLauncherPageAsync(LauncherPage page)
   {
      return RazorTemplateEngine.RenderAsync(
         "/Views/LauncherPage.cshtml", page);
   }

   private static Task<string> RenderFormPageAsync(FormPage page)
   {
      return RazorTemplateEngine.RenderAsync(
         "/Views/FormPage.cshtml", page);
   }

   private static Task<string> RenderContentPageAsync(ContentPage page)
   {
      return RazorTemplateEngine.RenderAsync(
         "/Views/ContentPage.cshtml", page);
   }
}
