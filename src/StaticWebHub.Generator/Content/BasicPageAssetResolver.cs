// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Rendering;

namespace StaticWebHub.Generator.Content;

internal sealed class BasicPageAssetResolver : IPageAssetResolver
{
   public BasicPageAssetResolver(
      IContentStore content,
      IAssetRenderer renderer)
   {
      ArgumentNullException.ThrowIfNull(content);
      ArgumentNullException.ThrowIfNull(renderer);
      _contentStore = content;
      _assetRenderer = renderer;
   }

   public Task<JsonNode> ResolveAsync(
      JsonNode? root,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(root);
      return ResolveNodeAsync(root, cancellationToken);
   }

   private async Task<JsonNode> ResolveNodeAsync(
      JsonNode node,
      CancellationToken cancellationToken)
   {
      cancellationToken.ThrowIfCancellationRequested();

      return node switch
      {
         JsonObject jsonObject => await ResolveObjectAsync(
            jsonObject, cancellationToken),
         JsonArray jsonArray => await ResolveArrayAsync(
            jsonArray, cancellationToken),
         JsonValue jsonValue => await ResolveValueAsync(
            jsonValue, cancellationToken),
         _ => node.DeepClone()
      };
   }

   private async Task<JsonNode> ResolveObjectAsync(
      JsonObject source,
      CancellationToken cancellationToken)
   {
      var result = new JsonObject();

      foreach (var property in source)
      {
         cancellationToken.ThrowIfCancellationRequested();
         if (property.Value is null)
         {
            result[property.Key] = null;
            continue;
         }

         result[property.Key] = await ResolveNodeAsync(
            property.Value, cancellationToken);
      }

      return result;
   }

   private async Task<JsonNode> ResolveArrayAsync(
      JsonArray source,
      CancellationToken cancellationToken)
   {
      var result = new JsonArray();

      foreach (var item in source)
      {
         cancellationToken.ThrowIfCancellationRequested();

         if (item is null)
         {
            result.Add(null);
            continue;
         }

         result.Add(await ResolveNodeAsync(item, cancellationToken));
      }

      return result;
   }

   private async Task<JsonNode> ResolveValueAsync(
      JsonValue source,
      CancellationToken cancellationToken)
   {
      if (source.TryGetValue<string>(out var text))
      {
         var resolved = await ResolveStringAsync(text, cancellationToken);
         return JsonValue.Create(resolved)!;
      }

      return source.DeepClone();
   }

   private async Task<string> ResolveStringAsync(
      string value,
      CancellationToken cancellationToken)
   {
      var matches = _assetPattern.Matches(value);
      if (matches.Count == 0)
      {
         return value;
      }

      var result = new StringBuilder();
      int position = 0;
      foreach (Match match in matches)
      {
         // Append everything from the pointer to the beginning of the match.
         result.Append(value, position, match.Index - position);

         var assetKey = match.Groups["key"].Value;
         var asset = await _contentStore.GetAssetAsync(
            assetKey, cancellationToken);
         result.Append(await _assetRenderer.RenderAsync(
            asset, cancellationToken));

         // Move the pointer to the end of the match.
         position = match.Index + match.Length;
      }

      // Copy everything from the pointer to the end of the string.
      // The third argument is the number of characters to copy. There are
      // length - position characters left in the string.
      result.Append(value, position, value.Length - position);
      return result.ToString();
   }

   private readonly IContentStore _contentStore;
   private readonly IAssetRenderer _assetRenderer;
   private static readonly Regex _assetPattern = new(
      @"\{\{(?<key>[^{}\r\n]+)\}\}",
      RegexOptions.Compiled | RegexOptions.CultureInvariant);
}