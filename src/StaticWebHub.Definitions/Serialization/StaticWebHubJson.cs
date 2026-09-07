using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StaticWebHub.Definitions.Serialization;

public class StaticWebHubJson
{
   public static JsonSerializerOptions CreateSerializerOptions()
   {
      var options = new JsonSerializerOptions
      {
         PropertyNameCaseInsensitive = true,
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         ReadCommentHandling = JsonCommentHandling.Skip,
         AllowTrailingCommas = true,
         AllowOutOfOrderMetadataProperties = true,
         TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver
      };

      options.Converters.Add(
         new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

      return options;
   }

   public static JsonDocumentOptions CreateDocumentOptions()
   {
      return new JsonDocumentOptions
      {
         CommentHandling = JsonCommentHandling.Skip,
         AllowTrailingCommas = true,
      };
   }
}
