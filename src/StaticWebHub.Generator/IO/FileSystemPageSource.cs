// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace StaticWebHub.Generator.IO;

internal sealed class FileSystemPageSource : IPageSource
{
   public const string DefaultPageSearchPattern = "*.page.json";

   public FileSystemPageSource(
      string rootPath,
      string pagePattern = DefaultPageSearchPattern)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
      _rootPath = Path.GetFullPath(rootPath);

      if (!Directory.Exists(_rootPath))
      {
         throw new DirectoryNotFoundException(
            $"Content root does not exist: {_rootPath}");
      }

      ArgumentException.ThrowIfNullOrWhiteSpace(pagePattern);
      _pagePattern = pagePattern;
   }

   public async IAsyncEnumerable<PageSource> GetPagesAsync(
      [EnumeratorCancellation]
         CancellationToken cancellationToken = default)
   {
      var paths = Directory.EnumerateFiles(
         _rootPath, _pagePattern, SearchOption.AllDirectories);
      foreach (var path in paths)
      {
         cancellationToken.ThrowIfCancellationRequested();
         yield return new PageSource()
         {
            FileName = Path.GetFileName(path),
            AbsolutePath = Path.GetFullPath(path, _rootPath),
            OpenReadAsync = (cancellationToken =>
            {
               cancellationToken.ThrowIfCancellationRequested();
               Stream stream = new FileStream(
                  path, FileMode.Open, FileAccess.Read, FileShare.Read,
                  4096, true);
               return ValueTask.FromResult(stream);
            }),
         };

         await Task.Yield();
      }
   }

   private readonly string _rootPath;

   private readonly string _pagePattern;

}
