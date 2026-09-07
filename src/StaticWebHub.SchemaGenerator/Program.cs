using System;
using System.CommandLine;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace StaticWebHub.SchemaGenerator;

internal static partial class Program
{
   public static async Task<int> Main(string[] args)
   {
      var checkOption = new Option<bool>("--check")
      {
         Description = "Verify existing schema files instead of generating them.",
      };

      var schemaDirectoryArgument = new Argument<DirectoryInfo?>("schema-directory")
      {
         Description = "Directory containing the schema files.",
         Arity = ArgumentArity.ZeroOrOne,
      };

      var schemaDirectoryOption = new Option<DirectoryInfo?>("--schema-directory", "-d")
      {
         Description = "Directory containing the schema files.",
         HelpName = "directory",
      };

      var rootCommand = new RootCommand("Generates JSON schemas for StaticWebHub definitions.");
      rootCommand.Options.Add(checkOption);
      rootCommand.Arguments.Add(schemaDirectoryArgument);
      rootCommand.Options.Add(schemaDirectoryOption);

      rootCommand.Validators.Add(result =>
      {
         var check = result.GetValue(checkOption);
         var argumentDirectory = result.GetValue(schemaDirectoryArgument);
         var optionDirectory = result.GetValue(schemaDirectoryOption);

         if (argumentDirectory is not null && optionDirectory is not null)
         {
            result.AddError(
               "Specify the schema directory either positionally " +
               "or with --schema-directory/-d, but not both.");
            return;
         }

         if (check && argumentDirectory is null && optionDirectory is null)
         {
            result.AddError(
               "A schema directory is required when --check is specified.");
         }
      });

      rootCommand.SetAction(async (parseResult, cancellationToken) =>
      {
         var check = parseResult.GetValue(checkOption);
         var schemaDirectory = parseResult.GetValue(schemaDirectoryOption) ??
            parseResult.GetValue(schemaDirectoryArgument);
         schemaDirectory ??= new DirectoryInfo("schemas");

         if (check)
         {
            return await CheckSchemasAsync(schemaDirectory, cancellationToken);
         }

         return await WriteSchemasAsync(schemaDirectory, cancellationToken);
      });

      var parseResult = rootCommand.Parse(args);
      return await parseResult.InvokeAsync();
   }

   private static async Task<int> CheckSchemasAsync(
      DirectoryInfo inputDirectory,
      CancellationToken cancellationToken)
   {
      try
      {
         if (!inputDirectory.Exists)
         {
            WriteError($"Schema directory does not exist: {inputDirectory.FullName}");
            return _exitFailure;
         }

         var pageFile = GetPageSchemaFile(inputDirectory);
         var siteFile = GetSiteConfigurationSchemaFile(inputDirectory);
         var filesExist = true;
         if (!pageFile.Exists)
         {
            WriteError($"Schema file does not exist: {pageFile.FullName}");
            filesExist = false;
         }

         if (!siteFile.Exists)
         {
            WriteError($"Schema file does not exist: {siteFile.FullName}");
            filesExist = false;
         }

         if (!filesExist)
         {
            return _exitFailure;
         }

         var generatedPageSchema = GeneratePageSchema();
         var generatedSiteSchema = GenerateSiteConfigurationSchema();
         var existingPageSchema = await ReadJsonAsync(pageFile, cancellationToken);
         var existingSiteSchema = await ReadJsonAsync(siteFile, cancellationToken);
         var schemasMatch = true;

         if (!JsonNode.DeepEquals(generatedPageSchema, existingPageSchema))
         {
            WriteError($"{pageFile.Name} is out of date.");
            schemasMatch = false;
         }

         if (!JsonNode.DeepEquals(generatedSiteSchema, existingSiteSchema))
         {
            WriteError($"{siteFile.Name} is out of date.");
            schemasMatch = false;
         }

         return schemasMatch ? _exitSuccess : _exitFailure;
      }
      catch (OperationCanceledException)
      {
         throw;
      }
      catch (Exception ex)
      {
         WriteError(ex.Message);
         return _exitFailure;
      }
   }

   private static void WriteError(string message)
   {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(message);
      Console.ResetColor();
   }

   private const int _exitSuccess = 0;
   private const int _exitFailure = 1;

   private const string _basicPageSchemaFileName = "page.schema.json";
   private const string _siteConfigurationFileName = "site.schema.json";
}