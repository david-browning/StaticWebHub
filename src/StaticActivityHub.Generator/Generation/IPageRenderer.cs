// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;
using StaticActivityHub.Generator.Models;

namespace StaticActivityHub.Generator.Generation;

internal interface IPageRenderer
{
   Task<string> GenerateAsync(
      BasicPage page,
      CancellationToken cancellationToken = default);
}
