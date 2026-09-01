// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StaticWebHub.Generator.Content;

public enum ContentWriteResults
{
   CreatedNew,
   Overwritten,
}

public interface IContentStore
{
   /// <summary>
   /// Gets asset metadata and text content, when applicable.
   /// </summary>
   /// <param name="assetKey">
   /// A root-relative logical key. Asset keys do not begin with a slash.
   /// </param>
   /// <param name="cancellationToken">
   /// A token used to cancel the content lookup operation.
   /// </param>
   /// <returns>
   /// Metadata and optional text content for the requested asset.
   /// </returns>
   Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default);

   Task<bool> AssetExistsAsync(
      string assetKey,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Opens a readable stream for an asset.
   /// </summary>
   Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default);

   Task<ContentWriteResults> WriteAsync(
      string assetKey,
      Stream contentStream,
      CancellationToken cancellationToken = default);

   Task DeleteFileAsync(
      string assetKey,
      CancellationToken cancellationToken = default);
}