// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Generation;

/// <summary>
/// Turns a BasicPage (or its polymorphic types) into an HTML document.
/// </summary>
internal interface IPageRenderer
{
   /// <summary>
   /// Converts a BasicPage (or its derived type) to an HTML document.
   /// </summary>
   /// <param name="page"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<string> RenderAsync(
      BasicPage page,
      CancellationToken cancellationToken = default);
}
