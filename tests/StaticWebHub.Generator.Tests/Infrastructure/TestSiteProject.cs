// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.IO;

namespace StaticWebHub.Generator.Tests.Infrastructure;

internal sealed class TestSiteProject : IDisposable
{
   public TestSiteProject()
   {
      _root = new TemporaryDirectory();

      ContentRoot = _root.CreateDirectory(
         Path.Combine("content", "en-us"));

      AssetRoot = _root.CreateDirectory("assets");
      OutputRoot = _root.GetPath("dist");

      _root.WriteText(
         "site.json",
         """
         {
           "title": "StaticWebHub Test Site",
           "defaultLocale": "en-us",
           "locales": [
             {
               "code": "en-us",
               "displayName": "English",
               "homePage": "/en-us/index/"
             }
           ]
         }
         """);
   }

   public string RootPath => _root.RootPath;

   public string ContentRoot { get; }

   public string AssetRoot { get; }

   public string OutputRoot { get; }

   public string WritePage(
      string fileName,
      string json)
   {
      return _root.WriteText(
         Path.Combine("content", "en-us", fileName),
         json);
   }

   public string WriteAsset(
      string relativePath,
      string content)
   {
      return _root.WriteText(
         Path.Combine("assets", relativePath),
         content);
   }

   public string GetOutputPath(params string[] parts)
   {
      var path = OutputRoot;

      foreach (var part in parts)
      {
         path = Path.Combine(path, part);
      }

      return path;
   }

   public void Dispose()
   {
      _root.Dispose();
   }

   private readonly TemporaryDirectory _root;
}
