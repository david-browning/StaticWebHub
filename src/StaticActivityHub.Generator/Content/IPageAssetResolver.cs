// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace StaticActivityHub.Generator.Content;

internal interface IPageAssetResolver
{
   Task<JsonNode> ResolveAsync(
      JsonNode? root,
      CancellationToken cancellationToken = default);
}