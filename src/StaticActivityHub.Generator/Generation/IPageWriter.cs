// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;
using StaticActivityHub.Generator.Models;

namespace StaticActivityHub.Generator.Generation;

internal interface IPageWriter
{
   Task WriteAsync(
      BasicPage page,
      string html,
      CancellationToken cancellationToken = default);
}
