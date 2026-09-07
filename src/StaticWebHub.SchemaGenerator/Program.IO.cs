using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace StaticWebHub.SchemaGenerator
{
   internal static partial class Program
   {
      private static FileInfo GetPageSchemaFile(
        DirectoryInfo directory)
      {
         return new FileInfo(
            Path.Combine(directory.FullName, _basicPageSchemaFileName));
      }

      private static FileInfo GetSiteConfigurationSchemaFile(
         DirectoryInfo directory)
      {
         return new FileInfo(
            Path.Combine(directory.FullName, _siteConfigurationFileName));
      }

      private static async Task<JsonNode> ReadJsonAsync(
         FileInfo file,
         CancellationToken cancellationToken)
      {
         await using var stream = file.OpenRead();
         return await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken) ??
            throw new JsonException($"Schema file was empty: {file.FullName}");
      }

      private static async Task<int> WriteSchemasAsync(
         DirectoryInfo outputDirectory,
         CancellationToken cancellationToken)
      {
         try
         {
            outputDirectory.Create();
            await WriteSchemaAsync(
               GeneratePageSchema(),
               GetPageSchemaFile(outputDirectory),
               cancellationToken);
            await WriteSchemaAsync(
               GenerateSiteConfigurationSchema(),
               GetSiteConfigurationSchemaFile(outputDirectory),
               cancellationToken);
         }
         catch (Exception ex)
         {
            WriteError(ex.Message);
            return _exitFailure;
         }

         return _exitSuccess;
      }

      private static async Task WriteSchemaAsync(
         JsonNode schema,
         FileInfo outputPath,
         CancellationToken cancellationToken)
      {
         var outputOptions = new JsonSerializerOptions
         {
            WriteIndented = true,
         };

         var json = schema.ToJsonString(outputOptions);
         await File.WriteAllTextAsync(
            outputPath.FullName, json, cancellationToken);
      }
   }
}
