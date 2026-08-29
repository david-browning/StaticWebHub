// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Markdig;

namespace StaticActivityHub.Generator.Content;

internal class BasicAssetRenderer : IAssetRenderer
{
   public BasicAssetRenderer()
   {
      _markdownPipeline = new MarkdownPipelineBuilder()
         .DisableHtml()
         .UseAdvancedExtensions()
         .Build();
   }

   public Task<string> RenderAsync(
      StoredAsset asset,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      var contentType = AssetHelpers.GetMediaType(asset.ContentType);
      string html;
      if (string.Equals(contentType, _htmlContentType, StringComparison.OrdinalIgnoreCase))
      {
         html = GetHtml(asset);
      }
      else if (string.Equals(contentType, _plainTextContentType, StringComparison.OrdinalIgnoreCase))
      {
         html = GetTextHtml(asset);
      }
      else if (string.Equals(contentType, _markdownContentType, StringComparison.OrdinalIgnoreCase))
      {
         html = GetMarkdownHtml(asset);
      }
      else if (contentType.StartsWith(
         _imageContentTypePrefix, StringComparison.OrdinalIgnoreCase))
      {
         html = GetImageHtml(asset);
      }
      else
      {
         throw new InvalidOperationException(
            $"Unsupported asset content type: {asset.ContentType}.");
      }

      return Task.FromResult(html);
   }

   private string GetHtml(StoredAsset asset)
   {
      // TODO: Sanitize the HTML.
      return GetAssetText(asset);
   }

   private string GetTextHtml(StoredAsset asset)
   {
      return ParagraphizeHtml(GetAssetText(asset));
   }

   private string GetMarkdownHtml(StoredAsset asset)
   {
      return Markdown.ToHtml(GetAssetText(asset), _markdownPipeline);
   }

   private string GetImageHtml(StoredAsset asset)
   {
      throw new NotImplementedException();
   }

   private string GetAssetText(StoredAsset asset)
   {
      if (asset.Text is null)
      {
         throw new InvalidOperationException(
            $"{asset.AssetKey} does not contain text.");
      }

      return asset.Text;
   }

   private string ParagraphizeHtml(string text)
   {
      var encoded = WebUtility.HtmlEncode(text);
      var paragraphs = encoded.Split(
         new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

      if (paragraphs.Length == 0)
      {
         return string.Empty;
      }

      return string.Join(
         string.Empty,
         Array.ConvertAll(paragraphs, paragraph => $"<p>{paragraph}</p>"));
   }

   private readonly MarkdownPipeline _markdownPipeline;
   private const string _htmlContentType = "text/html";
   private const string _plainTextContentType = "text/plain";
   private const string _markdownContentType = "text/markdown";
   private const string _imageContentTypePrefix = "image/";
}