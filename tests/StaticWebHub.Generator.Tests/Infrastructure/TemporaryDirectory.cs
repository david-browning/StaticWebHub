// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.IO;

namespace StaticWebHub.Generator.Tests.Infrastructure;

internal sealed class TemporaryDirectory : IDisposable
{
   public TemporaryDirectory()
   {
      RootPath = Path.Combine(
         Path.GetTempPath(),
         "StaticWebHub.Tests",
         Guid.NewGuid().ToString("N"));

      Directory.CreateDirectory(RootPath);
   }

   public string RootPath { get; }

   public string GetPath(params string[] parts)
   {
      var path = RootPath;

      foreach (var part in parts)
      {
         path = Path.Combine(path, part);
      }

      return path;
   }

   public string CreateDirectory(string relativePath)
   {
      var path = GetPath(relativePath);
      Directory.CreateDirectory(path);
      return path;
   }

   public string WriteText(
      string relativePath,
      string content)
   {
      var path = GetPath(relativePath);
      var parent = Path.GetDirectoryName(path);

      if (!string.IsNullOrEmpty(parent))
      {
         Directory.CreateDirectory(parent);
      }

      File.WriteAllText(path, content);
      return path;
   }

   public void Dispose()
   {
      if (Directory.Exists(RootPath))
      {
         Directory.Delete(
            RootPath,
            recursive: true);
      }
   }
}
