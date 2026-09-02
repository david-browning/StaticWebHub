// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Content;

namespace StaticWebHub.Generator.Tests.Infrastructure;

internal sealed class InMemoryContentStore : IContentStore
{
   public void AddTextAsset(
      string assetKey,
      string text,
      string? contentType = null)
   {
      AddAsset(
         assetKey,
         Encoding.UTF8.GetBytes(text),
         contentType ?? AssetHelpers.GetContentType(assetKey));
   }

   public void AddBinaryAsset(
      string assetKey,
      byte[] content,
      string? contentType = null)
   {
      AddAsset(
         assetKey,
         content,
         contentType ?? AssetHelpers.GetContentType(assetKey));
   }

   public Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();

      if (!_assets.TryGetValue(assetKey, out var asset))
      {
         throw new FileNotFoundException(
            $"Could not find test asset '{assetKey}'.",
            assetKey);
      }

      return Task.FromResult(asset.Asset);
   }

   public Task<bool> AssetExistsAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      return Task.FromResult(_assets.ContainsKey(assetKey));
   }

   public Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();

      if (!_assets.TryGetValue(assetKey, out var asset))
      {
         throw new FileNotFoundException(
            $"Could not find test asset '{assetKey}'.",
            assetKey);
      }

      Stream stream = new MemoryStream(
         asset.Content,
         writable: false);

      return Task.FromResult(stream);
   }

   public async Task<ContentWriteResults> WriteAsync(
      string assetKey,
      Stream contentStream,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(contentStream);

      var existed = _assets.ContainsKey(assetKey);

      using var buffer = new MemoryStream();
      await contentStream.CopyToAsync(buffer, cancellationToken);

      AddAsset(
         assetKey,
         buffer.ToArray(),
         AssetHelpers.GetContentType(assetKey));

      return existed ?
         ContentWriteResults.Overwritten :
         ContentWriteResults.CreatedNew;
   }

   public Task DeleteFileAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      _assets.Remove(assetKey);
      return Task.CompletedTask;
   }

   private void AddAsset(
      string assetKey,
      byte[] content,
      string contentType)
   {
      var lastModified =
         new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

      var text = AssetHelpers.IsTextContentType(contentType) ?
         Encoding.UTF8.GetString(content) :
         null;

      var asset = new StoredAsset
      {
         AssetKey = assetKey,
         ContentType = contentType,
         ContentLength = content.LongLength,
         Text = text,
         LastModifiedUtc = lastModified,
         EntityTag = AssetHelpers.GetEntityTag(
            assetKey,
            lastModified,
            content.LongLength),
      };

      _assets[assetKey] =
         new InMemoryAsset(
            asset,
            (byte[])content.Clone());
   }

   private sealed record InMemoryAsset(
      StoredAsset Asset,
      byte[] Content);

   private readonly Dictionary<string, InMemoryAsset> _assets =
      new(StringComparer.Ordinal);
}
