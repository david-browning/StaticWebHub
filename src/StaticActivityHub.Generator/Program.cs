// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StaticActivityHub.Generator.Content;
using StaticActivityHub.Generator.Generation;

namespace StaticActivityHub.Generator;

public static class Program
{
   public static async Task<int> Main(string[] args)
   {
      Console.WriteLine("Static Web Generator Copyright (C) 2026 4F Software LLC");

      var rootOption = new Option<DirectoryInfo>("--root", "-r")
      {
         Description = "Path to the static website project root.",
         Required = true
      };

      var contentOption = new Option<string>("--content", "-c")
      {
         Description = "Content directory, relative to the project root.",
         DefaultValueFactory = _ => "content"
      };

      var assetOption = new Option<string>("--assets", "-a")
      {
         Description = "Asset directory, relative to the project root",
         DefaultValueFactory = _ => "assets",
      };

      var outputOption = new Option<string>("--output", "-o")
      {
         Description = "Output directory, relative to the project root.",
         DefaultValueFactory = _ => "dist"
      };

      var pagePatternOption = new Option<string>("--page-pattern", "-p")
      {
         Description = "Filesystem search pattern used to find page definitions.",
         DefaultValueFactory = _ => "*.page.json"
      };

      var cleanOption = new Option<bool>("--clean")
      {
         Description = "Delete the output directory before generating the site."
      };

      var verboseOption = new Option<bool>("--verbose", "-v")
      {
         Description = "Write detailed generation information."
      };

      var rootCommand = new RootCommand(
         "Generates a static website from page definitions.");

      rootCommand.Options.Add(rootOption);
      rootCommand.Options.Add(contentOption);
      rootCommand.Options.Add(assetOption);
      rootCommand.Options.Add(outputOption);
      rootCommand.Options.Add(pagePatternOption);
      rootCommand.Options.Add(cleanOption);
      rootCommand.Options.Add(verboseOption);

      rootCommand.SetAction(async (parseResult, cancellationToken) =>
      {
         var root = parseResult.GetRequiredValue(rootOption);
         var content = parseResult.GetValue(contentOption) ?? "content";
         var assets = parseResult.GetValue(assetOption) ?? "assets";
         var output = parseResult.GetValue(outputOption) ?? "dist";
         var pagePattern = parseResult.GetValue(pagePatternOption) ?? "*.page.json";
         var clean = parseResult.GetValue(cleanOption);
         var verbose = parseResult.GetValue(verboseOption);
         return await GenerateAsync(
            root, content, assets, output, pagePattern,
            clean, verbose, cancellationToken);
      });

      var parseResult = rootCommand.Parse(args);

      return await parseResult.InvokeAsync();
   }

   private static async Task<int> GenerateAsync(
      DirectoryInfo projectRoot,
      string contentPath,
      string assetPath,
      string outputPath,
      string pagePattern,
      bool clean,
      bool verbose,
      CancellationToken cancellationToken)
   {
      if (!projectRoot.Exists)
      {
         Console.Error.WriteLine($"Project root does not exist: {projectRoot.FullName}");
         return 1;
      }

      var contentRoot = ResolvePath(projectRoot.FullName, contentPath);
      var assetRoot = ResolvePath(projectRoot.FullName, assetPath);
      var outputRoot = ResolvePath(projectRoot.FullName, outputPath);

      if (!Directory.Exists(contentRoot))
      {
         Console.Error.WriteLine($"Content directory does not exist: {contentRoot}");
         return 1;
      }

      if (clean && Directory.Exists(outputRoot))
      {
         if (verbose)
         {
            Console.WriteLine($"Cleaning output directory: {outputRoot}");
         }

         Directory.Delete(outputRoot, recursive: true);
      }

      Directory.CreateDirectory(outputRoot);

      if (verbose)
      {
         Console.WriteLine($"Project root: {projectRoot.FullName}");
         Console.WriteLine($"Content root: {contentRoot}");
         Console.WriteLine($"Output root: {outputRoot}");
         Console.WriteLine($"Page pattern: {pagePattern}");
      }

      IContentStore contentStore = new LocalContentStore(assetRoot);
      IAssetRenderer renderer = new BasicAssetRenderer();
      IPageAssetResolver resolver = new BasicPageAssetResolver(contentStore, renderer);
      IPageSource pageSource = new FileSystemPageSource(contentRoot, pagePattern);
      IPageDeserializer pageDeserializer = new BasicPageDeserializer(resolver);
      IPageRenderer pageGenerator = new BasicPageRenderer();
      IPageWriter pageWriter = new FileSystemPageWriter(outputRoot);
      var generator = new SiteGenerator(
         pageSource, pageDeserializer, pageGenerator, pageWriter);

      try
      {
         await generator.GenerateAsync(cancellationToken);
         CopyAssets(assetRoot, outputRoot);
         return 0;
      }
      catch (OperationCanceledException)
      {
         Console.Error.WriteLine("Generation cancelled.");
         return 2;
      }
      catch (Exception ex)
      {
         Console.Error.WriteLine($"Generation failed: {ex.Message}");

         if (verbose)
         {
            Console.Error.WriteLine(ex);
         }

         return 1;
      }
   }

   private static string ResolvePath(string projectRoot, string path)
   {
      return Path.GetFullPath(Path.IsPathRooted(path) ?
         path : Path.Combine(projectRoot, path));
   }

   private static void CopyAssets(string assetRoot, string outputRoot)
   {
      if (!Directory.Exists(assetRoot))
      {
         Console.WriteLine($"Could not find the assets folder {assetRoot}");
         return;
      }

      var assetOutputRoot = Path.Combine(outputRoot, "assets");
      CopyDirectory(assetRoot, assetOutputRoot);
   }

   private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
   {
      if (!Directory.Exists(sourceDirectory))
      {
         throw new DirectoryNotFoundException($"Asset directory does not exist: {sourceDirectory}");
      }

      Directory.CreateDirectory(destinationDirectory);

      foreach (var filePath in Directory.EnumerateFiles(sourceDirectory))
      {
         var fileName = Path.GetFileName(filePath);
         var destinationPath = Path.Combine(destinationDirectory, fileName);
         File.Copy(filePath, destinationPath, overwrite: true);
      }

      foreach (var directoryPath in Directory.EnumerateDirectories(sourceDirectory))
      {
         var directoryName = Path.GetFileName(directoryPath);
         var destinationPath = Path.Combine(destinationDirectory, directoryName);
         CopyDirectory(directoryPath, destinationPath);
      }
   }
}