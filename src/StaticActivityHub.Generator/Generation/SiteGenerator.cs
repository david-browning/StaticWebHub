// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;
using StaticActivityHub.Generator.Content;

namespace StaticActivityHub.Generator.Generation;

internal sealed class SiteGenerator
{
   public SiteGenerator(
       IPageSource pageSource,
       IPageDeserializer pageDeserializer,
       IPageRenderer pageRenderer,
       IPageWriter pageWriter)
   {
      _pageSource = pageSource;
      _pageDeserializer = pageDeserializer;
      _pageRenderer = pageRenderer;
      _pageWriter = pageWriter;
   }

   public async Task GenerateAsync(
      CancellationToken cancellationToken = default)
   {
      var pages = _pageSource.GetPagesAsync(cancellationToken);
      await foreach (var source in pages)
      {
         await using var input = await source.OpenReadAsync(
            cancellationToken);
         Models.BasicPage? page = null;
         try
         {
            page = await _pageDeserializer.DeserializeAsync(
               input, cancellationToken);
         }
         catch (Exception ex)
         {
            throw new InvalidOperationException(
               $"Could not deserialize page {source.AbsolutePath}", ex);
         }

         if (!page.IsPublished)
         {
            continue;
         }

         var html = await _pageRenderer.GenerateAsync(
            page, cancellationToken);
         await _pageWriter.WriteAsync(page, html, cancellationToken);
      }
   }

   private readonly IPageSource _pageSource;
   private readonly IPageDeserializer _pageDeserializer;
   private readonly IPageRenderer _pageRenderer;
   private readonly IPageWriter _pageWriter;
}
