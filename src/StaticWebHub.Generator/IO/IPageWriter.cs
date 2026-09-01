// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.IO;

/// <summary>
/// Writes the rendered HTML to a location.
/// The location and writing methodology are defined by classes that derive
/// from this.
/// </summary>
internal interface IPageWriter
{
   /// <summary>
   /// Write the HTML. The page provides metadata to help the write.
   /// </summary>
   /// <param name="page">Metadata the writer will use.</param>
   /// <param name="html">HTML document rendered by an IPageRenderer</param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task WriteAsync(
      BasicPage page,
      string html,
      CancellationToken cancellationToken = default);
}
