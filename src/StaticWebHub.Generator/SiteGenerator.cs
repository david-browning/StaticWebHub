// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Generation;
using StaticWebHub.Generator.IO;
using StaticWebHub.Generator.Serializing;
using StaticWebHub.Generator.Validation;

namespace StaticWebHub.Generator;

internal sealed class SiteGenerator
{
   public SiteGenerator(
       IPageSource pageSource,
       IPageDeserializer pageDeserializer,
       IPageValidator pageValidator,
       IPageRenderer pageRenderer,
       IPageWriter pageWriter)
   {
      _pageSource = pageSource;
      _pageDeserializer = pageDeserializer;
      _pageValidator = pageValidator;
      _pageRenderer = pageRenderer;
      _pageWriter = pageWriter;
   }

   public async Task<GenerationResult> GenerateAsync(
      CancellationToken cancellationToken = default)
   {
      var validationResults = new ValidationResult();
      var pages = _pageSource.GetPagesAsync(cancellationToken);
      await foreach (var source in pages)
      {
         await using var input = await source.OpenReadAsync(cancellationToken);
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

         validationResults.Combine(
            await _pageValidator.ValidateAsync(page, cancellationToken));
         if (!validationResults.HasErrors)
         {
            var html = await _pageRenderer.RenderAsync(
               page, cancellationToken);
            await _pageWriter.WriteAsync(page, html, cancellationToken);
         }
      }

      return new GenerationResult()
      {
         Validation = validationResults,
      };
   }

   private readonly IPageSource _pageSource;
   private readonly IPageDeserializer _pageDeserializer;
   private readonly IPageValidator _pageValidator;
   private readonly IPageRenderer _pageRenderer;
   private readonly IPageWriter _pageWriter;
}
