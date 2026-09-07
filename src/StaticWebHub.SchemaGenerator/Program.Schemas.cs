using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using StaticWebHub.Definitions.Models;
using StaticWebHub.Definitions.Serialization;

namespace StaticWebHub.SchemaGenerator;

internal static partial class Program
{
   private static JsonNode GeneratePageSchema()
   {
      return GenerateSchema(typeof(BasicPage));
   }

   private static JsonNode GenerateSiteConfigurationSchema()
   {
      return GenerateSchema(typeof(SiteConfiguration));
   }

   private static JsonNode GenerateSchema(Type type)
   {
      var jsonOptions = StaticWebHubJson.CreateSerializerOptions();
      var exporterOptions = new JsonSchemaExporterOptions
      {
         TreatNullObliviousAsNonNullable = true,
      };

      var schema = jsonOptions.GetJsonSchemaAsNode(type, exporterOptions);
      if (schema is not JsonObject root)
      {
         throw new InvalidOperationException(
            $"Schema for {type.Name} was not an object.");
      }

      root.Insert(0, "$schema", "https://json-schema.org/draft/2020-12/schema");
      return root;
   }
}
