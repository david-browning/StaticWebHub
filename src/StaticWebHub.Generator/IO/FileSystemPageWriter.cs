// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.IO;

internal class FileSystemPageWriter : IPageWriter
{
   public FileSystemPageWriter(string outputRoot)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(outputRoot);
      _outputRoot = Path.GetFullPath(outputRoot);
      Directory.CreateDirectory(_outputRoot);
   }

   public Task WriteAsync(
      BasicPage page,
      string html,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(page);
      ArgumentNullException.ThrowIfNull(html);

      var pageDir = GetPageDirectoryPath(page);
      var outputDir = Path.Combine(_outputRoot, pageDir);
      if (!Directory.Exists(outputDir))
      {
         Directory.CreateDirectory(outputDir);
      }

      var outputPath = Path.Combine(outputDir, "index.html");
      return File.WriteAllTextAsync(
         outputPath, html, Encoding.UTF8, cancellationToken);
   }

   private static string GetPageDirectoryPath(BasicPage page)
   {
      var slug = string.IsNullOrWhiteSpace(page.Slug) ?
         page.Id : page.Slug;
      return Path.Combine(page.Locale, slug);
   }

   private readonly string _outputRoot;
}
