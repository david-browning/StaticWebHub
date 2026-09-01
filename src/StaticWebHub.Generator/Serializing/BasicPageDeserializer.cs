// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Content;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Serializing;

internal class BasicPageDeserializer : IPageDeserializer
{
   public BasicPageDeserializer(IPageAssetResolver assetResolver)
   {
      ArgumentNullException.ThrowIfNull(assetResolver);
      _assetResolver = assetResolver;

      _options =
          new JsonSerializerOptions
          {
             PropertyNameCaseInsensitive = true,
             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
             ReadCommentHandling = JsonCommentHandling.Skip,
             AllowTrailingCommas = true,
             AllowOutOfOrderMetadataProperties = true,
          };

      _options.Converters.Add(
         new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

      _documentOptions = new JsonDocumentOptions
      {
         CommentHandling = JsonCommentHandling.Skip,
         AllowTrailingCommas = true
      };
   }

   public async Task<BasicPage> DeserializeAsync(
       Stream stream,
       CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(stream);

      var root = await JsonNode.ParseAsync(
         stream, documentOptions: _documentOptions,
         cancellationToken: cancellationToken) ??
         throw new JsonException("Page definition was empty.");
      var xformed = await _assetResolver.ResolveAsync(
         root, cancellationToken);
      var page = xformed.Deserialize<BasicPage>(_options);
      return page ?? throw new JsonException("Could not deserialize page.");
   }

   private readonly JsonSerializerOptions _options;
   private readonly IPageAssetResolver _assetResolver;
   private readonly JsonDocumentOptions _documentOptions;
}
