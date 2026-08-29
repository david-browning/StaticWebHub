// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Collections.Generic;
using System.Threading;

namespace StaticActivityHub.Generator.Content;

internal interface IPageSource
{
   IAsyncEnumerable<PageSource> GetPagesAsync(
      CancellationToken cancellationToken = default);
}
