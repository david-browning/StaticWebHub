// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Content;

namespace StaticWebHub.Generator.Rendering;

internal interface IAssetRenderer
{
   Task<string> RenderAsync(
      StoredAsset asset,
      CancellationToken cancellationToken = default);
}
