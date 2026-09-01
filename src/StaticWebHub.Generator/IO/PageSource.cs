// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StaticWebHub.Generator.IO;

internal sealed class PageSource
{
   public required string FileName { get; init; }

   public required string AbsolutePath { get; init; }

   public required Func<CancellationToken, ValueTask<Stream>> OpenReadAsync
   {
      get; init;
   }
}
